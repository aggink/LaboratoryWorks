using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.Entities;
using ProtectionOfInfo.WebApp.Extensions;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.InterfaceManager;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.Infrastructure.Services.PasswordValidatorsService;
using ProtectionOfInfo.WebApp.ViewModels.AccountViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.BookViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Controllers
{
    [AllowAnonymous]
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        // Авторизация
        // Смена пароля
        private readonly SignInManager<MyIdentityUser> _signInManager;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IMyPasswordValidatorService _passwordMyValidator;
        private readonly IBookManager _bookManager;

        public HomeController(
            UserManager<MyIdentityUser> userManager, 
            SignInManager<MyIdentityUser> signInManager, 
            ILogger<HomeController> logger, 
            IMapper mapper, 
            IMyPasswordValidatorService passwordMyValidator,
            IBookManager bookManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _passwordMyValidator = passwordMyValidator;
            _bookManager = bookManager;
        }

        #region NavigationPage

        public async Task<IActionResult> Index()
        {
            var operation = OperationResult.CreateResult<List<BookViewModel>>();

            var bookOperation = await _bookManager.GetListWithRelatedEntitiesAsync();
            operation.AppendLog(bookOperation.Logs);
            operation.MetaData = bookOperation.MetaData;
            operation.Exception = bookOperation.Exception;
            if (!bookOperation.Ok)
            { 
                return View(operation);
            }

            var bookViewModel = _mapper.Map<List<BookViewModel>>(bookOperation.Result);
            operation.Result = bookViewModel;
            return View(operation);
        }

        public IActionResult ProtectionOfInformation()
        {
            return View();
        }

        public IActionResult FundamentalsOfNewInformationTechnologies()
        {
            return View();
        }

        public IActionResult CourseWork()
        {
            return View();
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            if (User.IsInRole(AppData.AdministratorRoleName))
            {
                SetLayOutAdmin(2);
                return RedirectToAction("Index", "Administrator");
            }

            SetLayOutUser();
            return RedirectToAction("Index", "User");
        }

        #endregion

        #region LogIn_LogOut

        [HttpGet]
        public IActionResult CheckDbAccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if(user is null)
                {
                    ModelState.AddModelError("", "Данный пользователь не найден!");
                    return View(model);
                }

                if (user.BlockedUser)
                {
                    MyLoggerExtensions.UserAuntificated(_logger, $"Пользователь {user.UserName} ( ID: {user.Id } ) заблокирован!");
                    ModelState.AddModelError("", "Данный пользователь заблокирован!");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false); // <-- false - блокировка в случае ошибки входа

                if (user.FirstAccess)
                {
                    return RedirectToAction("UpdatePassword", "Home");
                }

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль!");
                    return View(model);
                }

                var passwordValid = _passwordMyValidator.Validate(model.Password!);
                if (user.PasswordValidation && !passwordValid.Succeeded)
                {
                    return RedirectToAction("UpdatePassword", "Home");
                }

                MyLoggerExtensions.UserAuntificated(_logger, $"Пользователь {user.UserName} ( ID: {user.Id } ) авторизован!");
                
                if (await _userManager.IsInRoleAsync(user, AppData.AdministratorRoleName))
                {
                    SetLayOutAdmin(2);
                    return RedirectToAction("Index", "Administrator");
                }

                if (await _userManager.IsInRoleAsync(user, AppData.UserRoleName))
                {
                    SetLayOutUser();
                    return RedirectToAction("Index", "User");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region UpdatePassword
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UpdatePassword()
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            MyIdentityUser? user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                string description = "Пользователь не найден!";
                return View("Error", description);
            }

            return View(_mapper.Map<UpdateUserViewModel>(user));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdatePassword(UpdateUserViewModel model)
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            if (ModelState.IsValid)
            {
                MyIdentityUser? user = await _userManager.GetUserAsync(User);

                if (user is null)
                {
                    ModelState.AddModelError("", "Пользователь не найден!");
                    //return RedirectToAction("UpdatePassword", "Home", model);
                    return View(model);
                }

                if (user.PasswordValidation)
                {
                    var passwordValid = _passwordMyValidator.Validate(model.NewPassword!);
                    if (!passwordValid.Succeeded)
                    {
                        foreach (var error in passwordValid.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }

                var VerificationResult = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.OldPassword);
                if (VerificationResult != PasswordVerificationResult.Success)
                {
                    ModelState.AddModelError("", "Старый пароль введен не правильно!");
                    return View(model);
                }

                var _passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<MyIdentityUser>)) as IPasswordValidator<MyIdentityUser>;
                var _passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<MyIdentityUser>)) as IPasswordHasher<MyIdentityUser>;

                if (_passwordHasher is null || _passwordValidator is null)
                {
                    ModelState.AddModelError("", "Ошибка при изменении пароля!");
                    return View(model);
                }

                var result = await _passwordValidator!.ValidateAsync(_userManager, user, model.NewPassword);
                if (!result.Succeeded)
                {
                    //foreach (var error in result.Errors)
                    //{
                    //    ModelState.AddModelError(string.Empty, error.Description);
                    //}
                    ModelState.AddModelError(string.Empty, "Пароль не удовлетворяет заданным ограничениям!");
                    return View(model);
                }

                user.PasswordHash = _passwordHasher!.HashPassword(user, model.NewPassword);

                if (user.FirstAccess)
                {
                    user.FirstAccess = false;
                }

                await _userManager.UpdateAsync(user);

                if (await _userManager.IsInRoleAsync(user, AppData.AdministratorRoleName))
                {
                    SetLayOutAdmin(2);
                    return RedirectToAction("Index", "Administrator");
                }
                
                if (await _userManager.IsInRoleAsync(user, AppData.UserRoleName))
                {
                    SetLayOutUser();
                    return RedirectToAction("Index", "User");
                }
            }

            return View(model);
        }

        #endregion

        private void SetLayOutAdmin(int NavBar = 0)
        {
            ViewBag.Admin = true;
            ViewBag.NavBar = NavBar;
        }
        private void SetLayOutUser()
        {
            ViewBag.User = true;
        }
    }
}
