using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.TelegramBot;
using ProtectionOfInfo.WebApp.TelegramBot.Interfaces;
using ProtectionOfInfo.WebApp.ViewModels.TelegramViewModel;
using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ProtectionOfInfo.WebApp.Controllers
{
    public class TelegramBotController : Controller
    {
        private readonly IFileTelegramManager _fileTelegramManager;
        private readonly IHandlerUpdateTelegramService _service;
        public TelegramBotController(
            IFileTelegramManager fileTelegramManager,
            IHandlerUpdateTelegramService service)
        {
            _fileTelegramManager = fileTelegramManager;
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            await _service.EchoAsync(update);
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = AppData.AdministratorRoleName)]
        [AutoValidateAntiforgeryToken]
        public IActionResult AddFile()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = AppData.AdministratorRoleName)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddFile(FileTelegramViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(nameof(AddFile), model);
                }

                byte[] data = new byte[model.UploadedFile!.Length];
                using (var stream = model.UploadedFile!.OpenReadStream())
                {
                    await stream.ReadAsync(data);
                }

                var file = new Data.Entities.ChatEntities.File()
                {
                    Data = data,
                    FileName = Path.GetFileNameWithoutExtension(model.UploadedFile!.FileName),
                    Extension = Path.GetExtension(model.UploadedFile!.FileName),
                    ContentType = model.UploadedFile.ContentType
                };

                await _fileTelegramManager.AddFileAsync(file, model.Description!, model.Value!, model.IsPublication);

                return RedirectToAction("Index", "Administrator");
            }
            catch
            {
                return View(nameof(AddFile), model);
            }
        }

        [HttpGet]
        [Authorize(Roles = AppData.AdministratorRoleName)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> GetFiles()
        {
            try
            {
                var result = await _fileTelegramManager.GetAllFilesAsync();
                if(result == null)
                {
                    throw new Exception("Файл не найден");
                }

                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Administrator");
            }
        }

        [HttpGet]
        [Authorize(Roles = AppData.AdministratorRoleName)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdateFile(string id)
        {
            try
            {
                var result = await _fileTelegramManager.GetFileWithInfoAsync(Guid.Parse(id));
                if (result == null)
                {
                    throw new Exception("Файлы не найдены");
                }

                return View(result);
            }
            catch
            {
                return RedirectToAction(nameof(GetFiles));
            }
        }

        [HttpPost]
        [Authorize(Roles = AppData.AdministratorRoleName)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdateFile(FileTelegramViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Description) || string.IsNullOrEmpty(model.Value))
                {
                    throw new Exception();
                }

                await _fileTelegramManager.UpdateFileAsync(Guid.Parse(model.Id!), model.Description!, model.Value!, model.IsPublication);
            }
            catch
            {
                return RedirectToAction(nameof(UpdateFile), model.Id);
            }

            return RedirectToAction(nameof(GetFiles));
        }

        [HttpGet]
        [Authorize(Roles = AppData.AdministratorRoleName)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteFile(string id)
        {
            try
            {
                await _fileTelegramManager.DeleteFileAsync(Guid.Parse(id));
                return RedirectToAction(nameof(GetFiles));
            }
            catch
            {
                return RedirectToAction(nameof(GetFiles));
            }
        }

        [HttpGet]
        [Authorize(Roles = AppData.AdministratorRoleName)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> GetFile(string id)
        {
            try
            {
                var file = await _fileTelegramManager.GetFileAsync(Guid.Parse(id));
                if(file == null)
                {
                    throw new Exception("Файл не найден");
                }

                return File(file.File, "application/" + file.Extension.Trim('.'), file.FileName + file.Extension);
            }
            catch
            {
                return RedirectToAction("Index", "Administrator");
            }
        }

    }
}
