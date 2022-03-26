using Calabonga.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.Entities;
using ProtectionOfInfo.WebApp.Data.Entities.ChatEntities;
using ProtectionOfInfo.WebApp.Data.Entities.CryptographyEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.InterfaceManager;
using ProtectionOfInfo.WebApp.ViewModels.ChatViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Hubs
{
    public class ChatMessagesManager : IChatMessagesManager
    {
        private readonly IUnitOfWork<ChatDbContext> _chatUnitOfWork;
        private readonly UserManager<MyIdentityUser> _userManager;

        public ChatMessagesManager(
            IUnitOfWork<ChatDbContext> unitOfWork,
            UserManager<MyIdentityUser> userManager)
        {
            _chatUnitOfWork = unitOfWork;
            _userManager = userManager;
        }

        private readonly string UrlFile = "/Chat/GetFile?id=";
        private readonly string UrlImage = "/Chat/GetImage?id=";

        private List<string> imagesExt = new List<string>()
        {
            "png", "jpeg", "bmp", "jpg"
        };

        public async Task<bool> AddMessageAsync(string userName, string message)
        {
            var chatRepository = _chatUnitOfWork.GetRepository<ChatMessage>();
            var _cancellationToken = new CancellationTokenSource().Token;
            await using var transaction = await _chatUnitOfWork.BeginTransactionAsync();

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new Exception("Пользователь не найден");
            }

            var entity = new ChatMessage()
            {
                Id = Guid.NewGuid(),
                CreatedBy = userName,
                UpdatedBy = userName,
                UserId = Guid.Parse(user.Id),
                Message = message,
            };

            await chatRepository.InsertAsync(entity, _cancellationToken);

            await _chatUnitOfWork.SaveChangesAsync();
            if (!_chatUnitOfWork.LastSaveChangesResult.IsOk)
            {
                await transaction.RollbackAsync(_cancellationToken);
                throw new Exception("Ошибка при отправке сообщения");
            }

            await transaction.CommitAsync(_cancellationToken);
            return true;
        }

        public async Task<List<MessageViewModel>> GetAllMessagesAsync()
        {
            var chatRepository = _chatUnitOfWork.GetRepository<ChatMessage>();
            var messages = await chatRepository.GetAllAsync(orderBy: x => x.OrderBy(i => i.UpdatedAt));
            if (messages == null)
            {
                throw new Exception("Ошибка при получении списка сообщений");
            }


            var result = new List<MessageViewModel>();
            for (int i = 0; i < messages.Count; i++)
            {
                var message = messages[i];

                var user = await _userManager.FindByIdAsync(message.UserId.ToString());
                if (user == null)
                {
                    throw new Exception("Пользователь не найден");
                }

                if (message.Message != null)
                {
                    var item = new MessageViewModel()
                    {
                        IsMessage = true,
                        UserName = user.UserName,
                        Message = message.Message
                    };

                    result.Add(item);
                }
                else if (message.IsImage)
                {
                    var item = new MessageViewModel()
                    {
                        IsImage = true,
                        UserName = user.UserName,
                        Url = UrlImage + message.Id.ToString(),
                        FileNameWithExtension = message.FileName + message.Extension
                    };

                    result.Add(item);
                }
                else
                {
                    var item = new MessageViewModel()
                    {
                        UserName = user.UserName,
                        Url = UrlFile + message.Id.ToString(),
                        FileNameWithExtension = message.FileName + message.Extension
                    };

                    result.Add(item);
                }
            }

            return result;
        }

        public async Task<FileUrlViewModel> AddFileAsync(string userName, FileDescription file)
        {
            var _cancellationToken = new CancellationTokenSource().Token;

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new Exception("Пользователь не найден");
            }

            //проверка на картинку
            var isImage = false;
            foreach (var ext in imagesExt)
            {
                if (file.Extension.Contains(ext, StringComparison.OrdinalIgnoreCase))
                {
                    isImage = true;
                    break;
                }
            }

            var message = new ChatMessage()
            {
                Id = Guid.NewGuid(),
                CreatedBy = user.UserName,
                UpdatedBy = user.UserName,
                UserId = Guid.Parse(user.Id),
                Message = null,
                ContentType = file.ContentType,
                FileName = file.FileName,
                Extension = file.Extension,
                Data = file.Data,
                IsImage = isImage,
            };

            var chatRepository = _chatUnitOfWork.GetRepository<ChatMessage>();
            await using var transaction = await _chatUnitOfWork.BeginTransactionAsync();

            await chatRepository.InsertAsync(message, _cancellationToken);

            await _chatUnitOfWork.SaveChangesAsync();
            if (!_chatUnitOfWork.LastSaveChangesResult.IsOk)
            {
                await transaction.RollbackAsync(_cancellationToken);
                throw new Exception("Ошибка при сохранении файла");
            }

            await transaction.CommitAsync(_cancellationToken);

            //обработка картинки
            if (isImage)
            {
                var imageUrl = UrlImage + message.Id;
                if (string.IsNullOrEmpty(imageUrl))
                {
                    await transaction.RollbackAsync(_cancellationToken);
                    throw new Exception("Ошибка при создании ссылки на картинку");
                }

                return new FileUrlViewModel()
                {
                    IsImage = true,
                    FileName = message.FileName,
                    UserName = user.UserName,
                    Url = imageUrl,
                    Extension = message.Extension
                };
            }

            //обработка файла
            var fileUrl = UrlFile + message.Id;
            if (string.IsNullOrEmpty(fileUrl))
            {
                await transaction.RollbackAsync(_cancellationToken);
                throw new Exception("Ошибка при создании ссылки на файл");
            }

            return new FileUrlViewModel()
            {
                FileName = message.FileName,
                UserName = user.UserName,
                Url = fileUrl,
                Extension = message.Extension
            };
        }

        public async Task<MyFileInfo> GetFileAsync(string id)
        {
            var repository = _chatUnitOfWork.GetRepository<ChatMessage>();
            var fileId = Guid.Parse(id);

            var file = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id == fileId);
            if (file == null || file.Data == null || file.FileName == null || file.Extension == null)
            {
                throw new Exception("Файл не найден");
            }

            return new MyFileInfo(file.Data, file.FileName, file.Extension);
        }

        public async Task<MyFileInfo> GetImageAsync(string id)
        {
            var repository = _chatUnitOfWork.GetRepository<ChatMessage>();
            var imageId = Guid.Parse(id);

            var file = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id == imageId);
            if (file != null && file.Data != null && file.FileName != null && file.Extension != null)
            {
                if (file.IsImage)
                {
                    return new MyFileInfo(file.Data, file.FileName, file.Extension);
                }
            }

            throw new Exception("Изображение не найдено");
        }

        public async Task<bool> DeleteMessageAsync()
        {
            var repository = _chatUnitOfWork.GetRepository<ChatMessage>();
            var messages = await repository.GetAllAsync(false);
            if (messages == null)
            {
                throw new Exception("Ошибка при получении сообщений");
            }

            repository.Delete(messages);

            await _chatUnitOfWork.SaveChangesAsync();
            if (!_chatUnitOfWork.LastSaveChangesResult.IsOk)
            {
                throw new Exception("Ошибка при очистке чата");
            }

            return true;
        }
    }
}
