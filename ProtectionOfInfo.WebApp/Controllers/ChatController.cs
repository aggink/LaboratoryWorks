using Calabonga.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.Entities;
using ProtectionOfInfo.WebApp.Data.Entities.ChatEntities;
using ProtectionOfInfo.WebApp.Hubs;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class ChatController : Controller
    {
        private readonly IUnitOfWork<ChatDbContext> _unitOfWork;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly CancellationToken _cancellationToken;
        private readonly IHubContext<CommunicationHub, ICommunicationHub> _hubContext;

        private List<string> imagesExt = new List<string>()
        {
            "png", "jpeg", "bmp", "jpg"
        };

        public ChatController(
            IUnitOfWork<ChatDbContext> unitOfWork, 
            UserManager<MyIdentityUser> userManager, 
            IHubContext<CommunicationHub, ICommunicationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _hubContext = hubContext;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        [HttpGet]
        public IActionResult Chat()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SendFile(IFormFile uploadedFile, string connectionId)
        {
            var opeartion = OperationResult.CreateResult<bool>();

            if (!ModelState.IsValid || uploadedFile == null || string.IsNullOrEmpty(connectionId))
            {
                opeartion.Result = false;
                opeartion.AddError("Ошибка при отправке файла");

                await _hubContext.Clients.Client(connectionId).SendErrorAsync(opeartion.MetaData.Message);

                return new JsonResult(opeartion);
            }

            try
            {
                byte[] data = new byte[uploadedFile.Length];
                using (var stream = uploadedFile.OpenReadStream())
                {
                    await stream.ReadAsync(data);
                }

                string userName = User?.Identity?.Name ?? "Anonymous";
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    opeartion.Result = false;
                    opeartion.AddError("Пользователь не найден");

                    await _hubContext.Clients.Client(connectionId).SendErrorAsync(opeartion.MetaData.Message);

                    return new JsonResult(opeartion);
                }

                var file = new FileDescription()
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = userName,
                    UpdatedBy = userName,
                    UserId = Guid.Parse(user.Id),
                    ContentType = uploadedFile.ContentType,
                    FileName = Path.GetFileNameWithoutExtension(uploadedFile.FileName),
                    Extension = Path.GetExtension(uploadedFile.FileName),
                    Data = data
                };

                var message = new ChatMessage()
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = userName,
                    UpdatedBy = userName,
                    UserId = Guid.Parse(user.Id),
                    Message = null,
                    FileId = file.Id
                };

                var fileRepository = _unitOfWork.GetRepository<FileDescription>();
                var chatRepository = _unitOfWork.GetRepository<ChatMessage>();
                await using var transaction = await _unitOfWork.BeginTransactionAsync();

                await fileRepository.InsertAsync(file, _cancellationToken);
                await chatRepository.InsertAsync(message, _cancellationToken);

                await _unitOfWork.SaveChangesAsync();
                if (!_unitOfWork.LastSaveChangesResult.IsOk)
                {
                    opeartion.Result = false;
                    opeartion.AddError("Ошибка при сохранении файла");

                    await transaction.RollbackAsync(_cancellationToken);
                    await _hubContext.Clients.Client(connectionId).SendErrorAsync(opeartion.MetaData.Message);

                    return new JsonResult(opeartion);
                }

                await transaction.CommitAsync(_cancellationToken);

                //обработка картинки
                foreach (var ext in imagesExt)
                {
                    if(file.Extension.Contains(ext, StringComparison.OrdinalIgnoreCase))
                    {
                        var imageUrl = $"/Chat/GetImage?id={file.Id}";
                        if (string.IsNullOrEmpty(imageUrl))
                        {
                            opeartion.Result = false;
                            opeartion.AddError("Ошибка при создании ссылки на картинку");

                            await transaction.RollbackAsync(_cancellationToken);
                            await _hubContext.Clients.Client(connectionId).SendErrorAsync(opeartion.MetaData.Message);
  
                            return new JsonResult(opeartion);
                        }

                        opeartion.Result = true;
                        opeartion.AddSuccess("Cсылка на изображения создана");

                        await _hubContext.Clients.All.SendImageAsync(user.UserName, imageUrl);

                        return new JsonResult(opeartion);
                    }
                }
                
                //обработка файла
                var fileUrl = $"/Chat/GetFile?id={file.Id}";
                if (string.IsNullOrEmpty(fileUrl))
                {
                    opeartion.Result = false;
                    opeartion.AddError("Ошибка при создании ссылки на файл");

                    await transaction.RollbackAsync(_cancellationToken);
                    await _hubContext.Clients.Client(connectionId).SendErrorAsync(opeartion.MetaData.Message);

                    return new JsonResult(opeartion);
                }

                opeartion.Result = true; 
                opeartion.AddSuccess("Cсылка на файл создана");

                await _hubContext.Clients.All.SendUrlAsync(user.UserName, fileUrl, file.FileName + file.Extension);

                return new JsonResult(opeartion);
            }
            catch (Exception ex)
            {
                opeartion.Result = false;
                opeartion.AddError(ex.Message);

                await _hubContext.Clients.Client(connectionId).SendErrorAsync(opeartion.MetaData.Message);

                return new JsonResult(opeartion);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFile(string id)
        {
            try
            {
                var fileRepository = _unitOfWork.GetRepository<FileDescription>();
                var fileId = Guid.Parse(id);
                var file = await fileRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == fileId);
                if(file != null)
                {
                    return File(file.Data, "application/" + file.Extension.Trim('.'), file.FileName + file.Extension);
                }

                return RedirectToAction(nameof(MessageError), new { message = "Ошибка получении файла" });
            }
            catch
            {
                return RedirectToAction(nameof(MessageError), new { message = "Ошибка получении файла" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetImage(string id)
        {
            try
            {
                var fileRepository = _unitOfWork.GetRepository<FileDescription>();
                var imageId = Guid.Parse(id);
                var file = await fileRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == imageId);
                if (file != null)
                {
                    foreach(var ext in imagesExt)
                    {
                        if(file.Extension.Contains(ext, StringComparison.OrdinalIgnoreCase))
                        {
                            return File(file.Data, "application/" + file.Extension.Trim('.'), id + file.Extension);
                        }
                    }
                }

                return RedirectToAction(nameof(MessageError), new {message = "Ошибка получении картинки"});
            }
            catch
            {
                return RedirectToAction(nameof(MessageError), new { message = "Ошибка получении картинки" });
            }
        }

        private JsonResult MessageError(string message)
        {
            var operation = OperationResult.CreateResult<string>();
            operation.AddError(message);
            return new JsonResult(operation);
        }

        [HttpGet]
        [Authorize(Roles = AppData.AdministratorRoleName)]
        public async Task<JsonResult> DeleteMessage()
        {
            var operation = OperationResult.CreateResult<bool>();

            var chatRepository = _unitOfWork.GetRepository<ChatMessage>();
            var fileRepository = _unitOfWork.GetRepository<FileDescription>();
            if(chatRepository != null && fileRepository != null)
            {
                var messages = await chatRepository.GetAllAsync(false);
                if (messages != null)
                {
                    foreach (var message in messages)
                    {
                        var file = await fileRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == message.Id);
                        if(file != null)
                        {
                            fileRepository.Delete(file);
                        }

                        chatRepository.Delete(message);
                    }

                    await _unitOfWork.SaveChangesAsync();                
                    if (_unitOfWork.LastSaveChangesResult.IsOk)
                    {
                        operation.AddError("Удалении сообщений завершилось успешно");
                        operation.Result = true;
                        return new JsonResult(operation);
                    }
                }
            }

            operation.AddError("Ошибка при удалении сообщений");
            operation.Result = false;
            return new JsonResult(operation);
        }
    }
}