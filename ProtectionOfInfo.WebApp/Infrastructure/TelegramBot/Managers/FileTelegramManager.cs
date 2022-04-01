using Calabonga.UnitOfWork;
using ProtectionOfInfo.WebApp.Data.DbContexts;
using ProtectionOfInfo.WebApp.Data.Entities.ChatEntities;
using ProtectionOfInfo.WebApp.Data.Entities.CryptographyEntities;
using ProtectionOfInfo.WebApp.Data.Entities.TelegramEntities;
using ProtectionOfInfo.WebApp.TelegramBot.Interfaces;
using ProtectionOfInfo.WebApp.ViewModels.TelegramViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.TelegramBot.Managers
{
    public class FileTelegramManager : IFileTelegramManager
    {
        public readonly IUnitOfWork<TelegramDbContext> _unitOfWork;
        public readonly IRepository<FileTelegram> _fileTelegramRepository;
        public FileTelegramManager(IUnitOfWork<TelegramDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _fileTelegramRepository = unitOfWork.GetRepository<FileTelegram>();
        }

        private readonly string UrlFile = "/TelegramBot/GetFile?id=";

        private List<string> imageExtension = new List<string>()
        {
            "png", "jpeg", "jpg"
        };

        public async Task<bool> AddFileAsync(File file, string description, string value, bool isPublication)
        {
            var _cancellationToken = new CancellationTokenSource().Token;
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            var isImage = false;
            foreach(var ext in imageExtension)
            {
                if (file.Extension.Contains(ext, StringComparison.OrdinalIgnoreCase))
                {
                    isImage = true;
                    break;
                }
            }

            var entity = new FileTelegram()
            {
                Id = Guid.NewGuid(),
                Data = file.Data,
                Description = description,
                IsPublication = isPublication,
                Value = value,
                IsImage = isImage,
                ContentType = file.ContentType,
                Extension = file.Extension,
                FileName = file.FileName,
            };

            await _fileTelegramRepository.InsertAsync(entity, _cancellationToken);

            await _unitOfWork.SaveChangesAsync();
            if (!_unitOfWork.LastSaveChangesResult.IsOk)
            {
                await transaction.RollbackAsync(_cancellationToken);
                throw new Exception("Ошибка при сохранении файла");
            }

            await transaction.CommitAsync(_cancellationToken);
            return true;
        }

        public async Task<bool> DeleteFileAsync(Guid id)
        {
            var entity = await _fileTelegramRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == id);
            if (entity == null)
            {
                throw new Exception("Файл не найден");
            }

            _fileTelegramRepository.Delete(entity);

            await _unitOfWork.SaveChangesAsync();
            if (!_unitOfWork.LastSaveChangesResult.IsOk)
            {
                throw new Exception("Ошибка при удалении файла");
            }

            return true;
        }

        public async Task<bool> UpdateFileAsync(Guid id, string description, string value, bool isPublication)
        {
            var entity = await _fileTelegramRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == id);
            if (entity == null)
            {
                throw new Exception("Файл не найден");
            }

            entity.Description = description;
            entity.Value = value;
            entity.IsPublication = isPublication;

            _fileTelegramRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            if (!_unitOfWork.LastSaveChangesResult.IsOk)
            {
                throw new Exception("Ошибка при сохранении файла");
            }

            return true;
        }

        public async Task<List<FileTelegramViewModel>> GetAllFilesAsync()
        {
            var entities = await _fileTelegramRepository.GetAllAsync(true);
            if (entities == null)
            {
                throw new Exception("Файл не найден");
            }

            var files = new List<FileTelegramViewModel>();
            foreach(var item in entities)
            {
                string url = UrlFile + item.Id.ToString(); ;

                files.Add(new FileTelegramViewModel()
                {
                    Id = item.Id.ToString(),
                    Description = item.Description,
                    Value = item.Value,
                    IsPublication = item.IsPublication,
                    IsImage = item.IsImage,
                    Url = url,
                    FileName = item.FileName + item.Extension
                });
            }

            return files;
        }

        public async Task<MyFileInfo> GetFileAsync(Guid id)
        {
            var image = await _fileTelegramRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == id);
            if(image == null)
            {
                throw new Exception("Файл не найден");
            }

            return new MyFileInfo(image.Data, image.FileName, image.Extension);
        }

        public async Task<FileTelegramViewModel> GetFileWithInfoAsync(Guid id)
        {
            var entity = await _fileTelegramRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == id);
            if (entity == null)
            {
                throw new Exception("Файл не найден");
            }

            string url = UrlFile + entity.Id.ToString();


            return new FileTelegramViewModel()
            {
                Id = entity.Id.ToString(),
                Description = entity.Description,
                Value = entity.Value,
                IsImage = entity.IsImage,
                IsPublication = entity.IsPublication,
                Url = url,
                FileName = entity.FileName + entity.Extension
            };
        }
    }
}
