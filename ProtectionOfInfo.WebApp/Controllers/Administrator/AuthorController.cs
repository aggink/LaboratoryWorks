using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.Base;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.Infrastructure.Repositories.Base;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.AuthorViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Controllers.Administrator
{
    [Authorize(Roles = AppData.AdministratorRoleName)]
    [AutoValidateAntiforgeryToken]
    public class AuthorController : Controller
    {
        private readonly IMyRepository<Author> _repository;
        private readonly IMyManager<Author> _manager;
        private readonly IMapper _mapper;
        public AuthorController(
            IMyRepository<Author> repository,
            IMyManager<Author> manager,
            IMapper mapper)
        {
            _repository = repository;
            _manager = manager;
            _mapper = mapper;
        }

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AuthorCreateViewModel model)
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            if (!ModelState.IsValid)
            {
                return View(OperationResult.CreateResult<AuthorCreateViewModel>(model));
            }

            var mapModel = _mapper.Map<Author>(model);
            mapModel.CreatedBy = User.Identity!.Name!;
            mapModel.UpdatedBy = User.Identity!.Name!;

            var entity = await _repository.CreateAsync(mapModel);
            if (!entity.Ok)
            {
                var operation = OperationResult.CreateResult<AuthorCreateViewModel>(model);
                operation.AppendLog(entity.Logs);
                operation.MetaData = entity.MetaData;
                operation.Exception = entity.Exception;
                return View(operation);
            }

            return RedirectToAction(nameof(Authors), await GetListEntitiesAsync(entity.Logs));
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
                var list = await GetListEntitiesAsync(
                    new List<string>()
                    {
                        "Id задан неверно"
                    },
                    "Id задан неверно",
                    MetaDataType.Info);

                return View(nameof(Authors), list);
            }

            OperationResult<Author> entity = await _repository.GetByIdAsync(guidId);
            if (!entity.Ok)
            {
                var list = await GetListEntitiesAsync(entity.Logs, entity.MetaData.Message, entity.MetaData.Type);
                return View(nameof(Authors), list);
            }

            var operation = OperationResult.CreateResult<AuthorUpdateViewModel>(_mapper.Map<AuthorUpdateViewModel>(entity.Result));
            operation.AppendLog(entity.Logs);
            operation.MetaData = entity.MetaData;
            return View(operation);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AuthorUpdateViewModel model)
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            if (!ModelState.IsValid)
            {
                return View(OperationResult.CreateResult<AuthorUpdateViewModel>(model));
            }

            var mapModel = _mapper.Map<Author>(model);
            mapModel.UpdatedBy = User.Identity!.Name!;

            var entity = await _repository.UpdateAsync(mapModel);
            if (!entity.Ok)
            {
                var operation = OperationResult.CreateResult<AuthorUpdateViewModel>(model);
                operation.AppendLog(entity.Logs);
                operation.MetaData = entity.MetaData;
                operation.Exception = entity.Exception;
                return View(operation);
            }

            return View(nameof(Authors), await GetListEntitiesAsync(entity.Logs));
        }

        #endregion

        #region Delete

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
                return View(nameof(Authors), operation);
            }

            var deleteAuthors = await _repository.DeleteAsync(guidId);
            if (!deleteAuthors.Ok)
            {
                var operation = await GetListEntitiesAsync(deleteAuthors.Logs);
                return View(nameof(Authors), operation);
            }

            return View(nameof(Authors), await GetListEntitiesAsync());
        }

        #endregion

        #region List

        [HttpGet]
        public async Task<IActionResult> Authors()
        {
            return View(await GetListEntitiesAsync());
        }

        #endregion

        private async Task<OperationResult<List<AuthorViewModel>>> GetListEntitiesAsync(IEnumerable<string>? logs = null, string? message = null, MetaDataType? type = null)
        {
            var operation = OperationResult.CreateResult<List<AuthorViewModel>>();
            if (logs != null)
            {
                operation.AppendLog(logs);
            }

            var authors = await _repository.GetListAsync();
            operation.AppendLog(authors.Logs);
            operation.Exception = authors.Exception;
            if (message != null && type != null && authors.Ok)
            {
                operation.MetaData = new MetaData(operation, message, (MetaDataType)type);
            }
            else
            {
                operation.MetaData = authors.MetaData;
            }

            if (authors.Ok)
            {
                List<AuthorViewModel> authorsViewModel = _mapper.Map<List<AuthorViewModel>>(authors.Result);
                operation.Result = authorsViewModel;
            }

            return operation;
        }

        #region SearchAndSort

        [HttpGet]
        public async Task<IActionResult> Search(string ObjectSearch, string Search)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(Search))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Запрос поиска пустой" }, "Запрос поиска пустой!", MetaDataType.Error);
                    return View(nameof(Authors), list);
                }
                if (string.IsNullOrEmpty(ObjectSearch))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Объект поиска не задан!" }, "Объект поиска не задан!", MetaDataType.Error);
                    return View(nameof(Authors), list);
                }

                var searchResult = await _manager.SearchAsync(ObjectSearch, Search);

                var operation = OperationResult.CreateResult<List<AuthorViewModel>>();
                operation.AppendLog(searchResult.Logs);
                operation.MetaData = searchResult.MetaData;
                operation.Exception = searchResult.Exception;
                if (searchResult.Ok)
                {
                    operation.Result = _mapper.Map<List<AuthorViewModel>>(searchResult.Result);
                }

                return View(nameof(Authors), operation);

            }

            return View(nameof(Authors), await GetListEntitiesAsync(new List<string> { "Запрос завершился с ошибкой!" }, "Запрос завершился с ошибкой!", MetaDataType.Info));
        }

        [HttpGet]
        public async Task<IActionResult> Sort(string ObjectSort, string TypeSort)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(TypeSort))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Не задан тип сортировки!" }, "Не задан тип сортировки!", MetaDataType.Error);
                    return View(nameof(Authors), list);
                }
                if (string.IsNullOrEmpty(ObjectSort))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Не задан объект сортировки!" }, "Не задан объект сортировки!", MetaDataType.Error);
                    return View(nameof(Authors), list);
                }

                var sortResult = await _manager.SortAsync(ObjectSort, TypeSort);

                var operation = OperationResult.CreateResult<List<AuthorViewModel>>();
                operation.AppendLog(sortResult.Logs);
                operation.MetaData = sortResult.MetaData;
                operation.Exception = sortResult.Exception;
                if (sortResult.Ok)
                {
                    operation.Result = _mapper.Map<List<AuthorViewModel>>(sortResult.Result);
                }

                return View(nameof(Authors), operation);
            }

            return View(nameof(Authors), await GetListEntitiesAsync(new List<string> { "Запрос завершился с ошибкой!" }, "Запрос завершился с ошибкой!", MetaDataType.Info));
        }

        #endregion
    }
}
