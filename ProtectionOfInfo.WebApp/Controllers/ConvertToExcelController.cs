using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Infrastructure.Services.ConvertToExcelService;
using System;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class ConvertToExcelController : Controller
    {
        private readonly IConvertToExcel _convertToExcel;

        public ConvertToExcelController(IConvertToExcel convertToExcel)
        {
            _convertToExcel = convertToExcel;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetBooksExcel()
        {
            var excelData = Task.Run(async () => await _convertToExcel.ConvertBookToExcel()).Result;
            if (excelData == null || excelData.Length == 0)
            {
                return RedirectToAction(nameof(Index));
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
                return RedirectToAction(nameof(Index));
            }

            string fileName = "DataBase.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(excelData, contentType, fileName);
        }
    }
}
