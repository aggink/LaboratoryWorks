using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.InterfaceManager;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.Infrastructure.Repositories.Base;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.BookViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Controllers.Administrator
{
    [Authorize(Roles = AppData.AdministratorRoleName)]
    [AutoValidateAntiforgeryToken]
    public class BookController : Controller
    {
        private readonly IMyRepository<Book> _repository;
        private readonly IBookManager _manager;
        private readonly IMapper _mapper;
        public BookController(
            IMyRepository<Book> bookRepository,
            IBookManager manager,
            IMapper mapper)
        {
            _repository = bookRepository;
            _manager = manager;
            _mapper = mapper;
        }

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            return View(await _manager.GetEmptyEntitiesWithParamAsync(new BookCreateViewModel()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookCreateViewModel model)
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            if (!ModelState.IsValid)
            {
                var operation = await _manager.GetEmptyEntitiesWithParamAsync(model);
                return View(operation);
            }

            var checkParam = await _manager.СheckRelationshipsWithOtherEntitiesAsync(model);
            if (!checkParam.Ok)
            {
                var operation = await _manager.GetEmptyEntitiesWithParamAsync(model);
                return View(operation);
            }

            var mapModel = _mapper.Map<Book>(model);
            mapModel.CreatedBy = User.Identity!.Name!;
            mapModel.UpdatedBy = User.Identity!.Name!;

            var entity = await _repository.CreateAsync(mapModel);
            if (!entity.Ok)
            {
                var operation = await _manager.GetEmptyEntitiesWithParamAsync(model);
                operation.AppendLog(entity.Logs);
                operation.MetaData = entity.MetaData;
                operation.Exception = entity.Exception;

                return View(operation);
            }

            return RedirectToAction(nameof(Books), await GetListEntitiesAsync(entity.Logs));
        }

        #endregion

        #region Update

        [HttpGet]
        public async Task<IActionResult> Update(string Id)
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            Guid guidId;
            if (!Guid.TryParse(Id, out guidId))
            {
                return View(nameof(Books), await GetListEntitiesAsync(new List<string>() { "Id задан неверно" }));
            }

            var book = await _repository.GetByIdAsync(guidId);
            if (!book.Ok)
            {
                return View(nameof(Books), await GetListEntitiesAsync(book.Logs, book.MetaData.Message, MetaDataType.Info));
            }

            var operation = await _manager.GetEmptyEntitiesWithParamAsync(_mapper.Map<BookUpdateViewModel>(book.Result));
            if (!operation.Ok)
            {
                return View(nameof(Books), await GetListEntitiesAsync(operation.Logs, operation.MetaData.Message, MetaDataType.Info));
            }

            return View(operation);
        }

        [HttpPost]
        public async Task<IActionResult> Update(BookUpdateViewModel model)
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            if (!ModelState.IsValid)
            {
                var operation = await _manager.GetEmptyEntitiesWithParamAsync(model);
                return View(operation);
            }

            var checkParam = await _manager.СheckRelationshipsWithOtherEntitiesAsync(model);
            if (!checkParam.Ok)
            {
                var operation = await _manager.GetEmptyEntitiesWithParamAsync(model);
                return View(operation);
            }

            var mapModel = _mapper.Map<Book>(model);
            mapModel.UpdatedBy = User.Identity!.Name!;

            var entity = await _repository.UpdateAsync(mapModel);
            if (!entity.Ok)
            {
                var operation = await _manager.GetEmptyEntitiesWithParamAsync(model);
                operation.AppendLog(entity.Logs);
                operation.MetaData = entity.MetaData;
                operation.Exception = entity.Exception;
                return View(operation);
            }

            return View(nameof(Books), await GetListEntitiesAsync(entity.Logs));
        }

        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            Guid guidId;
            if (!Guid.TryParse(Id, out guidId))
            {
                var operation = await GetListEntitiesAsync(new List<string>() { "Id задан неверно" });
                return View(nameof(Books), operation);
            }

            var deleteAuthors = await _repository.DeleteAsync(guidId);
            if (!deleteAuthors.Ok)
            {
                var operation = await GetListEntitiesAsync(deleteAuthors.Logs);
                return View(nameof(Books), operation);
            }

            return View(nameof(Books), await GetListEntitiesAsync());
        }

        #endregion

        #region List

        [HttpGet]
        public async Task<IActionResult> Books()
        {
            return View(await GetListEntitiesAsync());
        }

        #endregion

        #region OtherMethods
        private async Task<OperationResult<BooksViewModel>> GetListEntitiesAsync(IEnumerable<string>? logs = null, string? message = null, MetaDataType? type = null)
        {
            var operation = OperationResult.CreateResult<BooksViewModel>();
            if (logs != null)
            {
                operation.AppendLog(logs);
            }

            var books = await _repository.GetListAsync();
            operation.AppendLog(books.Logs);
            operation.Exception = books.Exception;
            if (message != null && type != null && books.Ok)
            {
                operation.MetaData = new MetaData(operation, message, (MetaDataType)type);
            }
            else
            {
                operation.MetaData = books.MetaData;
            }

            if (books.Ok)
            {
                List<BookViewModel> booksViewModel = _mapper.Map<List<BookViewModel>>(books.Result);

                var maxResult = await _manager.MaxBookPrice();
                var minResult = await _manager.MinBookPrice();

                operation.AppendLog(maxResult.Logs);
                operation.AppendLog(minResult.Logs);

                var viewModel = new BooksViewModel()
                {
                    Books = booksViewModel,
                    MaxPrice = maxResult.Result,
                    MinPrice = minResult.Result
                };

                operation.Result = viewModel;
            }

            return operation;
        }

        #endregion

        #region SearchAndSort

        [HttpGet]
        public async Task<IActionResult> Search(string ObjectSearch, string Search)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(Search))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Запрос поиска пустой" }, "Запрос поиска пустой!", MetaDataType.Error);
                    return View(nameof(Books), list);
                }
                if (string.IsNullOrEmpty(ObjectSearch))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Объект поиска не задан!" }, "Объект поиска не задан!", MetaDataType.Error);
                    return View(nameof(Books), list);
                }

                var searchResult = await _manager.SearchAsync(ObjectSearch, Search);

                var operation = OperationResult.CreateResult<BooksViewModel>();
                operation.AppendLog(searchResult.Logs);
                operation.MetaData = searchResult.MetaData;
                operation.Exception = searchResult.Exception;
                if (searchResult.Ok)
                {
                    List<BookViewModel> booksViewModel = _mapper.Map<List<BookViewModel>>(searchResult.Result);

                    var maxResult = await _manager.MaxBookPrice();
                    var minResult = await _manager.MinBookPrice();

                    operation.AppendLog(maxResult.Logs);
                    operation.AppendLog(minResult.Logs);

                    var viewModel = new BooksViewModel()
                    {
                        Books = booksViewModel,
                        MaxPrice = maxResult.Result,
                        MinPrice = minResult.Result
                    };

                    operation.Result = viewModel;
                }

                return View(nameof(Books), operation);

            }

            return View(nameof(Books), await GetListEntitiesAsync(new List<string> { "Запрос завершился с ошибкой!" }, "Запрос завершился с ошибкой!", MetaDataType.Info));
        }

        [HttpGet]
        public async Task<IActionResult> Sort(string ObjectSort, string TypeSort)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(TypeSort))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Не задан тип сортировки!" }, "Не задан тип сортировки!", MetaDataType.Error);
                    return View(nameof(Books), list);
                }
                if (string.IsNullOrEmpty(ObjectSort))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Не задан объект сортировки!" }, "Не задан объект сортировки!", MetaDataType.Error);
                    return View(nameof(Books), list);
                }

                var sortResult = await _manager.SortAsync(ObjectSort, TypeSort);

                var operation = OperationResult.CreateResult<BooksViewModel>();
                operation.AppendLog(sortResult.Logs);
                operation.MetaData = sortResult.MetaData;
                operation.Exception = sortResult.Exception;
                if (sortResult.Ok)
                {
                    List<BookViewModel> booksViewModel = _mapper.Map<List<BookViewModel>>(sortResult.Result);

                    var maxResult = await _manager.MaxBookPrice();
                    var minResult = await _manager.MinBookPrice();

                    operation.AppendLog(maxResult.Logs);
                    operation.AppendLog(minResult.Logs);

                    var viewModel = new BooksViewModel()
                    {
                        Books = booksViewModel,
                        MaxPrice = maxResult.Result,
                        MinPrice = minResult.Result
                    };

                    operation.Result = viewModel;
                }

                return View(nameof(Books), operation);
            }

            return View(nameof(Books), await GetListEntitiesAsync(new List<string> { "Запрос завершился с ошибкой!" }, "Запрос завершился с ошибкой!", MetaDataType.Info));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchWithOtherEntities(string ObjectSearch, string Search)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(Search))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Запрос поиска пустой" }, "Запрос поиска пустой!", MetaDataType.Error);
                    return View("../Home/Index", list);
                }
                if (string.IsNullOrEmpty(ObjectSearch))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Объект поиска не задан!" }, "Объект поиска не задан!", MetaDataType.Error);
                    return View("../Home/Index", list);
                }

                var searchResult = await _manager.SearchWithOtherEntitiesAsync(ObjectSearch, Search);

                var operation = OperationResult.CreateResult<List<BookViewModel>>();
                operation.AppendLog(searchResult.Logs);
                operation.MetaData = searchResult.MetaData;
                operation.Exception = searchResult.Exception;
                if (searchResult.Ok)
                {
                    operation.Result = _mapper.Map<List<BookViewModel>>(searchResult.Result);
                }

                return View("../Home/Index", operation);

            }

            var error = OperationResult.CreateResult<List<BookViewModel>>();
            var entities = await _manager.GetListWithRelatedEntitiesAsync();
            error.AppendLog(entities.Logs);
            error.Exception = entities.Exception;
            if (entities.Ok)
            {
                error.AddInfo("Запрос завершился с ошибкой!");
                error.Result = _mapper.Map<List<BookViewModel>>(entities.Result);
            }

            return View("../Home/Index", error);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SortWithOtherEntities(string ObjectSort, string TypeSort)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(TypeSort))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Не задан тип сортировки!" }, "Не задан тип сортировки!", MetaDataType.Error);
                    return View("../Home/Index", list);
                }
                if (string.IsNullOrEmpty(ObjectSort))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Не задан объект сортировки!" }, "Не задан объект сортировки!", MetaDataType.Error);
                    return View("../Home/Index", list);
                }

                var sortResult = await _manager.SortWithOtherEntitiesAsync(ObjectSort, TypeSort);

                var operation = OperationResult.CreateResult<List<BookViewModel>>();
                operation.AppendLog(sortResult.Logs);
                operation.MetaData = sortResult.MetaData;
                operation.Exception = sortResult.Exception;
                if (sortResult.Ok)
                {
                    operation.Result = _mapper.Map<List<BookViewModel>>(sortResult.Result);
                }

                return View("../Home/Index", operation);
            }

            var error = OperationResult.CreateResult<List<BookViewModel>>();
            var entities = await _manager.GetListWithRelatedEntitiesAsync();
            error.AppendLog(entities.Logs);
            error.Exception = entities.Exception;
            if (entities.Ok)
            {
                error.AddInfo("Запрос завершился с ошибкой!");
                error.Result = _mapper.Map<List<BookViewModel>>(entities.Result);
            }

            return View("../Home/Index", error);
        }

        #endregion
    }
}
