using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService.Interface;
using System;
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
        public FileEncryptionController(ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService;
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

            if (CheckValid(operation, uploadedFile, password, algorithm))
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

            if (CheckValid(operation, uploadedFile, password, ""))
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

        private bool CheckValid(OperationResult<FileContentResult> operation, IFormFile uploadedFile, string password, string algorithm)
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
    }
}
