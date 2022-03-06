using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Controllers.User
{
    /// <summary>
    ///  // aggink: update summary - 27.02.2022 17:03:07
    /// </summary>
    [AllowAnonymous]
    [AutoValidateAntiforgeryToken]
    public class FileEncryptionController : Controller
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly ISettingEDSFileService _settingEDSFileService;
        public FileEncryptionController(ICryptographyService cryptographyService, ISettingEDSFileService settingEDSFileService)
        {
            _cryptographyService = cryptographyService;
            _settingEDSFileService = settingEDSFileService;
        }

        [HttpGet]
        public IActionResult FileEncryption()
        {
            return View();
        }

        #region Encrypt/DeDecrypt File

        [HttpPost]
        public async Task<IActionResult> EncryptFile(IFormFile uploadedFile, string algorithm, string password)
        {
            var operation = OperationResult.CreateResult<FileContentResult>();

            if (CheckValidEncrypt(operation, uploadedFile, password, algorithm))
            {
                return View(nameof(FileEncryption), operation);
            }

            try
            {
                string fileName = uploadedFile!.FileName;
                string ext = Path.GetExtension(fileName);
                string fileType = "application/" + ext.Trim('.');
                fileName = "EncryptFile" + ext;

                byte[] data = new byte[uploadedFile.Length];
                using (var stream = uploadedFile.OpenReadStream())
                {
                    await stream.ReadAsync(data);
                }
                
                var encrypt = await Task.Run(() => _cryptographyService.Encrypt(data, algorithm!, password));
                if (encrypt.Length < data.Length)
                {
                    operation.AddError("В процессе шифрования файла произошла ошибка");
                    return View(nameof(FileEncryption), operation);
                }

                return File(encrypt, fileType, fileName);
            }
            catch
            {
                operation.AddError("В процессе шифрования файла произошла ошибка");
                return View(nameof(FileEncryption), operation);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DecryptFile(IFormFile uploadedFile, string password)
        {
            var operation = OperationResult.CreateResult<FileContentResult>();

            if (CheckValidEncrypt(operation, uploadedFile, password, ""))
            {
                return View(nameof(FileEncryption), operation);
            }

            try
            {
                string fileName = uploadedFile!.FileName;
                string ext = Path.GetExtension(fileName);
                string fileType = "application/" + ext.Trim('.');
                fileName = "DecryptFile" + ext;

                byte[] data = new byte[uploadedFile.Length];
                using (var stream = uploadedFile.OpenReadStream())
                {
                    await stream.ReadAsync(data);
                }

                var encrypt = await Task.Run(() => _cryptographyService.Decrypt(data, password));
                if(encrypt.Length == 0)
                {
                    operation.AddError("В процессе расшифровки файла произошла ошибка");
                    return View(nameof(FileEncryption), operation);
                }

                return File(encrypt, fileType, fileName);
            }
            catch
            {
                operation.AddError("В процессе расшифровки файла произошла ошибка");
                return View(nameof(FileEncryption), operation);
            }
        }

        private bool CheckValidEncrypt(OperationResult<FileContentResult> operation, IFormFile uploadedFile, string password, string algorithm)
        {
            bool error = false;

            if (uploadedFile == null)
            {
                operation.AddError("Файл не задан");
                error = true;
            }

            if (algorithm == null)
            {
                operation.AddError("Алгоритм шифрования не задан");
                error = true;
            }

            if (string.IsNullOrEmpty(password))
            {
                operation.AddError("Пароль не задан");
                error = true;
            }

            if (!ModelState.IsValid)
            {
                operation.AddError("Ошибка в обработке входных данных");
                error = true;
            }

            return error;
        }

        #endregion

        #region SignFiles 

        public IActionResult DigitalSignature()
        {
            return View();
        }

        public async Task<IActionResult> CreateСertificate(int months = 3)
        {
            var operation = OperationResult.CreateResult<bool>();
            if(!ModelState.IsValid)
            {
                operation.AddError("Ошибка при обработке введенных данных");
                return View(nameof(DigitalSignature), operation);
            }

            var archive = await _settingEDSFileService.CreateСertificateAsync(months);
            if(archive == null)
            {
                operation.AddError("При создании сертификата произошла ошибка");
                return View(nameof(DigitalSignature), operation);
            }

            return File(archive.File, $"application/{archive.Extension}", archive.FileName);
        }

        public async Task<IActionResult> SignFile(IFormFile fileForSign, IFormFile privateKey, string passwordForSign)
        {
            var operation = CheckValidSign(fileForSign, privateKey, passwordForSign);
            if (!operation.Ok)
            {
                return View(nameof(DigitalSignature), operation);
            }

            var signFile = await _settingEDSFileService.SignFileAsync(fileForSign, privateKey, passwordForSign);
            if(signFile == null)
            {
                operation.AddError("При подписании файла произошла ошибка");
                return View(nameof(DigitalSignature), operation);
            }

            return File(signFile.File, $"application/{signFile.Extension}", signFile.FileName);
        }

        public async Task<IActionResult> CheckSignFile(IFormFile fileForCheck, IFormFile publicKey, string passwordForCheck)
        {
            var operation = CheckValidSign(fileForCheck, publicKey, passwordForCheck);
            if (!operation.Ok)
            {
                return View(nameof(DigitalSignature), operation);
            }

            var checkSign = await _settingEDSFileService.CheckSignFileAsync(fileForCheck, publicKey, passwordForCheck);
            if(checkSign)
            {
                operation.AddSuccess("Файл прошел проверку ЭЦП");
            }
            else
            {
                operation.AddError("Файл не прошел проверку ЭЦП");
            }

            return View(nameof(DigitalSignature), operation);
        }

        public async Task<IActionResult> GetOriginalFile(IFormFile fileForDelete)
        {
            var operation = OperationResult.CreateResult<bool>();
            if (!ModelState.IsValid)
            {
                operation.AddError("Ошибка при обработке введенных данных");
                return View(nameof(DigitalSignature), operation);
            }

            if (fileForDelete == null)
            {
                operation.AddError("Файл для подписания не найден");
                return View(nameof(DigitalSignature), operation);
            }

            var originFile = await _settingEDSFileService.GetOriginalFileAsync(fileForDelete);
            if (originFile == null)
            {
                operation.AddError("При преобразовании файла к исходному виду произошла ошибка");
                operation.Result = false;
                return View(nameof(DigitalSignature), operation);
            }

            return File(originFile.File, $"application/{originFile.Extension}", originFile.FileName);
        }

        private OperationResult<bool> CheckValidSign(IFormFile file, IFormFile key, string password)
        {
            var operation = OperationResult.CreateResult<bool>();
            operation.Result = true;

            if (!ModelState.IsValid)
            {
                operation.AddError("Ошибка при обработке введенных данных");
                operation.Result = false;
            }

            if (file == null)
            {
                operation.AddError("Файл для подписания не найден");
                operation.Result = false;
            }

            if (key == null)
            {
                operation.AddError("Файл с приватным ключом не найден");
                operation.Result = false;
            }

            if (String.IsNullOrEmpty(password))
            {
                operation.AddError("Пароль не задан");
                operation.Result = false;
            }

            return operation;
        }

        #endregion
    }
}
