using Calabonga.UnitOfWork;
using OfficeOpenXml;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Data.Configurations;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.InterfaceManager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.ConvertToExcelService
{
    /// <summary>
    ///  // aggink: update summary - 02.03.2022 22:25:49
    /// </summary>
    public class ConvertToExcel : IConvertToExcel
    {
        private readonly IBookManager _bookManager;
        private readonly IUnitOfWork<CatalogDbContext> _unitOfWork;

        public ConvertToExcel(IBookManager bookManager, IUnitOfWork<CatalogDbContext> unitOfWork)
        {
            _bookManager = bookManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<byte[]> ConvertBookToExcel()
        {
            var result = await _bookManager.GetListWithRelatedEntitiesAsync();
            if (!result.Ok) return new byte[0];
            var books = result.Result;

            try
            {
                byte[] excelData;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Books");
                    sheet.Cells[1, 1].Value = EntitiesModelsConfiguration.BookName;
                    sheet.Cells[1, 2].Value = EntitiesModelsConfiguration.BookPrice;
                    sheet.Cells[1, 3].Value = EntitiesModelsConfiguration.AuthorName;
                    sheet.Cells[1, 4].Value = EntitiesModelsConfiguration.CategoryName;
                    sheet.Cells[1, 5].Value = EntitiesModelsConfiguration.PublisherName;

                    for (int i = 0; i < books.Count; i++)
                    {
                        sheet.Cells[i + 2, 1].Value = books[i].Name;
                        sheet.Cells[i + 2, 2].Value = books[i].Price;
                        sheet.Cells[i + 2, 3].Value = books[i].Author!.Name;
                        sheet.Cells[i + 2, 4].Value = books[i].Category!.Name;
                        sheet.Cells[i + 2, 5].Value = books[i].Publisher!.Name;
                    }

                    excelData = await package.GetAsByteArrayAsync();
                }

                return excelData;
            }
            catch
            {
                return new byte[0];
            }
        }

        public async Task<byte[]> ConvertDbToExcel()
        {
            var authors = await _unitOfWork.GetRepository<Author>().GetAllAsync(false);
            var books = await _unitOfWork.GetRepository<Book>().GetAllAsync(false);
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(false);
            var publishers = await _unitOfWork.GetRepository<Publisher>().GetAllAsync(false);

            bool error = false;
            if(authors == null) error = true;
            if(books == null) error = true;
            if(categories == null) error = true;
            if(publishers == null) error = true;

            if(error) return new byte[0];

            try
            {
                byte[] excelData;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {

                    var authorsSheet = package.Workbook.Worksheets.Add("Authors");
                    authorsSheet.Cells[1, 1].Value = "Id";
                    authorsSheet.Cells[1, 2].Value = EntitiesModelsConfiguration.AuthorName;
                    authorsSheet.Cells[1, 3].Value = EntitiesModelsConfiguration.AuthorBiography;

                    Parallel.For(0, authors!.Count,  i =>
                    {
                        authorsSheet.Cells[i + 2, 1].Value = authors[i].Id;
                        authorsSheet.Cells[i + 2, 2].Value = authors[i].Name;
                        authorsSheet.Cells[i + 2, 3].Value = authors[i].Biography;
                    });

                    var booksSheet = package.Workbook.Worksheets.Add("Books");
                    booksSheet.Cells[1, 1].Value = "Id";
                    booksSheet.Cells[1, 2].Value = EntitiesModelsConfiguration.BookAuthorId;
                    booksSheet.Cells[1, 3].Value = EntitiesModelsConfiguration.BookCategoryId;
                    booksSheet.Cells[1, 4].Value = EntitiesModelsConfiguration.BookPublisherId;
                    booksSheet.Cells[1, 5].Value = EntitiesModelsConfiguration.BookName;
                    booksSheet.Cells[1, 6].Value = EntitiesModelsConfiguration.BookPrice;

                    Parallel.For(0, books!.Count, i =>
                    {
                        booksSheet.Cells[i + 2, 1].Value = books[i].Id;
                        booksSheet.Cells[i + 2, 2].Value = books[i].AuthorId;
                        booksSheet.Cells[i + 2, 3].Value = books[i].CategoryId;
                        booksSheet.Cells[i + 2, 4].Value = books[i].PublisherId;
                        booksSheet.Cells[i + 2, 5].Value = books[i].Name;
                        booksSheet.Cells[i + 2, 6].Value = books[i].Price;
                    });

                    var categoriesSheet = package.Workbook.Worksheets.Add("Categories");
                    categoriesSheet.Cells[1, 1].Value = "Id";
                    categoriesSheet.Cells[1, 2].Value = EntitiesModelsConfiguration.CategoryName;
                    categoriesSheet.Cells[1, 3].Value = EntitiesModelsConfiguration.CategorySynopsis;

                    Parallel.For(0, categories!.Count, i =>
                    {
                        categoriesSheet.Cells[i + 2, 1].Value = categories[i].Id;
                        categoriesSheet.Cells[i + 2, 2].Value = categories[i].Name;
                        categoriesSheet.Cells[i + 2, 3].Value = categories[i].Synopsis;
                    });

                    var publishersSheet = package.Workbook.Worksheets.Add("Publishers");
                    publishersSheet.Cells[1, 1].Value = "Id";
                    publishersSheet.Cells[1, 2].Value = EntitiesModelsConfiguration.PublisherName;
                    publishersSheet.Cells[1, 3].Value = EntitiesModelsConfiguration.PublisherDescription;

                    Parallel.For(0, publishers!.Count, i =>
                    {
                        publishersSheet.Cells[i + 2, 1].Value = publishers[i].Id;
                        publishersSheet.Cells[i + 2, 2].Value = publishers[i].Name;
                        publishersSheet.Cells[i + 2, 3].Value = publishers[i].Description;
                    });

                    excelData = await package.GetAsByteArrayAsync();
                }

                return excelData;
            }
            catch
            {
                return new byte[0];
            }
        }
    }
}
