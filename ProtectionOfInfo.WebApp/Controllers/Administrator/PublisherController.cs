using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.Base;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.Infrastructure.Repositories.Base;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.PublisherViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Controllers.Administrator
{
    [Authorize(Roles = AppData.AdministratorRoleName)]
    [AutoValidateAntiforgeryToken]
    public class PublisherController : Controller
    {
        private readonly IMyRepository<Publisher> _repository;
        private readonly IMyManager<Publisher> _manager;
        private readonly IMapper _mapper;
        public PublisherController(IMyRepository<Publisher> repository, IMyManager<Publisher> manager, IMapper mapper)
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
        public async Task<IActionResult> Create(PublisherCreateViewModel model)
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            if (!ModelState.IsValid)
            {
                return View(OperationResult.CreateResult<PublisherCreateViewModel>(model));
            }

            var mapModel = _mapper.Map<Publisher>(model);
            mapModel.CreatedBy = User.Identity!.Name!;
            mapModel.UpdatedBy = User.Identity!.Name!;

            var entity = await _repository.CreateAsync(mapModel);
            if (!entity.Ok)
            {
                var operation = OperationResult.CreateResult<PublisherCreateViewModel>(model);
                operation.AppendLog(entity.Logs);
                operation.MetaData = entity.MetaData;
                operation.Exception = entity.Exception;
                return View(operation);
            }

            return RedirectToAction(nameof(Publishers), await GetListEntitiesAsync(entity.Logs));
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

                return View(nameof(Publishers), list);
            }

            OperationResult<Publisher> entity = await _repository.GetByIdAsync(guidId);
            if (!entity.Ok)
            {
                var list = await GetListEntitiesAsync(entity.Logs, entity.MetaData.Message, entity.MetaData.Type);
                return View(nameof(Publishers), list);
            }

            var operation = OperationResult.CreateResult<PublisherUpdateViewModel>(_mapper.Map<PublisherUpdateViewModel>(entity.Result));
            operation.AppendLog(entity.Logs);
            operation.MetaData = entity.MetaData;
            return View(operation);
        }

        [HttpPost]
        public async Task<IActionResult> Update(PublisherUpdateViewModel model)
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            if (!ModelState.IsValid)
            {
                return View(OperationResult.CreateResult<PublisherUpdateViewModel>(model));
            }

            var mapModel = _mapper.Map<Publisher>(model);
            mapModel.UpdatedBy = User.Identity!.Name!;

            var entity = await _repository.UpdateAsync(mapModel);
            if (!entity.Ok)
            {
                var operation = OperationResult.CreateResult<PublisherUpdateViewModel>(model);
                operation.AppendLog(entity.Logs);
                operation.MetaData = entity.MetaData;
                operation.Exception = entity.Exception;
                return View(operation);
            }

            return View(nameof(Publishers), await GetListEntitiesAsync(entity.Logs));
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
                return View(nameof(Publishers), operation);
            }

            var deleteAuthors = await _repository.DeleteAsync(guidId);
            if (!deleteAuthors.Ok)
            {
                var operation = await GetListEntitiesAsync(deleteAuthors.Logs);
                return View(nameof(Publishers), operation);
            }

            return View(nameof(Publishers), await GetListEntitiesAsync());
        }

        #endregion

        #region List

        [HttpGet]
        public async Task<IActionResult> Publishers()
        {
            return View(await GetListEntitiesAsync());
        }

        #endregion

        private async Task<OperationResult<List<PublisherViewModel>>> GetListEntitiesAsync(IEnumerable<string>? logs = null, string? message = null, MetaDataType? type = null)
        {
            var operation = OperationResult.CreateResult<List<PublisherViewModel>>();
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
                List<PublisherViewModel> authorsViewModel = _mapper.Map<List<PublisherViewModel>>(authors.Result);
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
                    return View(nameof(Publishers), list);
                }
                if (string.IsNullOrEmpty(ObjectSearch))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Объект поиска не задан!" }, "Объект поиска не задан!", MetaDataType.Error);
                    return View(nameof(Publishers), list);
                }

                var searchResult = await _manager.SearchAsync(ObjectSearch, Search);

                var operation = OperationResult.CreateResult<List<PublisherViewModel>>();
                operation.AppendLog(searchResult.Logs);
                operation.MetaData = searchResult.MetaData;
                operation.Exception = searchResult.Exception;
                if (searchResult.Ok)
                {
                    operation.Result = _mapper.Map<List<PublisherViewModel>>(searchResult.Result);
                }

                return View(nameof(Publishers), operation);

            }

            return View(nameof(Publishers), await GetListEntitiesAsync(new List<string> { "Запрос завершился с ошибкой!" }, "Запрос завершился с ошибкой!", MetaDataType.Info));
        }

        [HttpGet]
        public async Task<IActionResult> Sort(string ObjectSort, string TypeSort)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(TypeSort))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Не задан тип сортировки!" }, "Не задан тип сортировки!", MetaDataType.Error);
                    return View(nameof(Publishers), list);
                }
                if (string.IsNullOrEmpty(ObjectSort))
                {
                    var list = await GetListEntitiesAsync(new List<string> { "Не задан объект сортировки!" }, "Не задан объект сортировки!", MetaDataType.Error);
                    return View(nameof(Publishers), list);
                }

                var sortResult = await _manager.SortAsync(ObjectSort, TypeSort);

                var operation = OperationResult.CreateResult<List<PublisherViewModel>>();
                operation.AppendLog(sortResult.Logs);
                operation.MetaData = sortResult.MetaData;
                operation.Exception = sortResult.Exception;
                if (sortResult.Ok)
                {
                    operation.Result = _mapper.Map<List<PublisherViewModel>>(sortResult.Result);
                }

                return View(nameof(Publishers), operation);
            }

            return View(nameof(Publishers), await GetListEntitiesAsync(new List<string> { "Запрос завершился с ошибкой!" }, "Запрос завершился с ошибкой!", MetaDataType.Info));
        }

        #endregion
    }
}
