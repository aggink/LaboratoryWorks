using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.InterfaceManager;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.Infrastructure.Services.ConvertToExcelService;
using ProtectionOfInfo.WebApp.Infrastructure.Services.ConvertToWordService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class ConvertToFileController : Controller
    {
        private readonly IConvertToExcel _convertToExcel;
        private readonly IConvertToWord _convertToWord;
        private readonly IBookManager _bookManager;

        public ConvertToFileController(IConvertToExcel convertToExcel, IConvertToWord convertToWord, IBookManager bookManager)
        {
            _convertToExcel = convertToExcel;
            _convertToWord = convertToWord;
            _bookManager = bookManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetBooksExcel()
        {
            var excelData = Task.Run(async () => await _convertToExcel.ConvertBookToExcel()).Result;
            if (excelData == null || excelData.Length == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string fileName = "BooksPrice.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(excelData, contentType, fileName);
        }

        [HttpGet]
        [Authorize(Roles = AppData.AdministratorRoleName)]
        public IActionResult GetDbExcel()
        {
            var excelData = Task.Run(async () => await _convertToExcel.ConvertDbToExcel()).Result;
            if (excelData == null || excelData.Length == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string fileName = "DataBase.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(excelData, contentType, fileName);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCheck()
        {
            var bookOperation = await _bookManager.GetListWithRelatedEntitiesAsync();
            if (!bookOperation.Ok)
            {
                return RedirectToAction("Index", "Home");
            }

            var products = new List<Product>();
            Random rnd = new Random();
            for (int i = 0; i < bookOperation.Result.Count; i++)
            {
                products.Add(new Product()
                {
                    Id = bookOperation.Result[i].Id,
                    VendorCode = bookOperation.Result[i].Id.ToString().Split('-')[0],
                    Name = $"Книга \"{bookOperation.Result[i].Name}\"",
                    Amount = rnd.Next(1, 60),
                    Unit = "шт",
                    Price = bookOperation.Result[i].Price,
                    Discount = rnd.Next(15)
                });
            }

            var check = new Check()
            {
                Type = "ТС",
                Number = rnd.Next(111111, 999999),
                Date = DateTime.Now,
                Supplier = "Интернет-магазина \"КомБук\", ИНН 772400322680, ОГРНИП 310774629801461, 117105, г. Москва, 1-й Нагатинский пр., д.6., стр.1",
                SupplierDetails = "р/с 40802810938060022945 в ПАО \"СБЕРБАНК\", г. Москва БИК 044525225, к/с 30101810400000000225",
                Сustomer = "Физическое лицо",
                DeliveryAddress = "г. Москва, ул. Народная, д. 4, стр. 1, оф. 111",
                DeliveryType = "Самовывоз",
                Products = products
            };

            var operation = OperationResult.CreateResult<bool>();

            var file = Task.Run(async () => await _convertToWord.CreateDocumentAsync(check)).Result;
            if(file == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return File(file.File, $"application/{file.Extension}", file.FileName);
        }
    }
}
