using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.Infrastructure.Services.DictionaryApiService;
using ProtectionOfInfo.WebApp.ViewModels.DictionaryViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Controllers
{
    [AllowAnonymous]
    [AutoValidateAntiforgeryToken]
    public class DictionaryController : Controller
    {
        private readonly IDictionaryApi _dictionatyApi;
        private readonly IMapper _mapper;
        public DictionaryController(IDictionaryApi dictionaryApi, IMapper mapper)
        {
            _dictionatyApi = dictionaryApi;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string word, string code)
        {
            var operation = OperationResult.CreateResult<DictionaryViewModel>();
            bool error = false;

            if (string.IsNullOrEmpty(word))
            {
                error = true;
                operation.AddError("Слово для поиска не задано");
            }

            if (string.IsNullOrEmpty(code))
            {
                error = true;
                operation.AddError("Код языка для поиска слова не задан");
            }

            if (error)
            {
                return View(nameof(Index), operation);
            }

            var result = await _dictionatyApi.GetWordDefinitionsAsync(word, code);
            operation.AppendLog(result.Logs);
            operation.MetaData = result.MetaData;
            operation.Exception = result.Exception;
            if (!result.Ok)
            {
                return View(nameof(Index), operation);
            }

            var dictionaryViewModel = _mapper.Map<DictionaryViewModel>(result.Result);
            operation.Result = dictionaryViewModel;
            return View(nameof(Index), operation);
        }

        [HttpGet]
        public async  Task<IActionResult> GetJsonSearch(string word, string code)
        {
            var operation = OperationResult.CreateResult<string>();
            bool error = false;

            if (string.IsNullOrEmpty(word))
            {
                error = true;
                operation.AddError("Слово для поиска не задано");
            }

            if (string.IsNullOrEmpty(code))
            {
                error = true;
                operation.AddError("Код языка для поиска слова не задан");
            }

            if (error)
            {
                return View(nameof(Index), operation);
            }

            var result = await _dictionatyApi.GetJsonWordDefinitionsAsync(word, code);
            operation.AppendLog(result.Logs);
            operation.MetaData = result.MetaData;
            if (!result.Ok)
            {
                operation.Exception = result.Exception;
                return View(nameof(Index), operation);
            }

            ViewBag.JSON = true;
            operation.Result = result.Result;
            return View(operation);
        }
    }
}
