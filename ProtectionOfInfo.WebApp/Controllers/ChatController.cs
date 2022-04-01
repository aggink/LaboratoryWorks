using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Hubs;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.TelegramBot;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class ChatController : Controller
    {
        private readonly IChatMessagesManager _chatMessagesManager;
        private readonly IHubContext<CommunicationHub, ICommunicationHub> _hubContext;
        private readonly IHandlerUpdateTelegramService _handlerTelegramService;

        public ChatController(
            IChatMessagesManager chatMessagesManager,
            IHubContext<CommunicationHub, ICommunicationHub> hubContext,
            IHandlerUpdateTelegramService handlerTelegramService)
        {
            _chatMessagesManager = chatMessagesManager;
            _hubContext = hubContext;
            _handlerTelegramService = handlerTelegramService;
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
                var file = new Data.Entities.ChatEntities.File()
                {
                    ContentType = uploadedFile.ContentType,
                    FileName = Path.GetFileNameWithoutExtension(uploadedFile.FileName),
                    Extension = Path.GetExtension(uploadedFile.FileName),
                    Data = data
                };

                var result = await _chatMessagesManager.AddFileAsync(userName, file);

                if (result.IsImage)
                {
                    await _hubContext.Clients.All.SendImageAsync(result.UserName, result.Url);
                    await Task.Run(() => _handlerTelegramService.SendAllClientImage(file.Data, file.FileName, file.Extension, $"от {userName}"));
                }
                else
                {
                    await _hubContext.Clients.All.SendUrlAsync(result.UserName, result.Url, result.FileName + result.Extension);
                    await Task.Run(() => _handlerTelegramService.SendAllClientFile(file.Data, file.FileName, file.Extension, $"от {userName}"));
                }

                opeartion.Result = true;
                opeartion.AddSuccess("Запрос успешно обработан");

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
                var file = await _chatMessagesManager.GetFileAsync(id);
                if(file == null)
                {
                    throw new Exception("Файл не найден");
                }

                return File(file.File, "application/" + file.Extension.Trim('.'), file.FileName + file.Extension);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(MessageError), new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetImage(string id)
        {
            try
            {
                var image = await _chatMessagesManager.GetImageAsync(id);
                if(image == null)
                {
                    throw new Exception("Файл не найден");
                }

                return File(image.File, "application/" + image.Extension.Trim('.'), id + image.Extension);

            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(MessageError), new { message = ex.Message });
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

            try
            {
                var result = await _chatMessagesManager.DeleteMessageAsync();
                if (!result)
                {
                    throw new Exception("Ошибка при очистке чата");
                }

                operation.AddSuccess("Удалении сообщений завершилось успешно");
                operation.Result = true;
                return new JsonResult(operation);
            }
            catch(Exception ex)
            {
                operation.AddError(ex.Message);
                operation.Result = false;
                return new JsonResult(operation);
            }
        }
    }
}