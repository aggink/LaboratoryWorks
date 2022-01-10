using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.Entities;
using ProtectionOfInfo.WebApp.Infrastructure.Services.PasswordValidatorsService;
using ProtectionOfInfo.WebApp.ViewModels.AccountViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Controllers.Administrator
{
    [Authorize(Roles = AppData.AdministratorRoleName)]
    [AutoValidateAntiforgeryToken]
    public class AdministratorController : Controller
    {
        // Просмотр списка имен зарегистрированных пользователей (всего целиком)
        // Установление для пользователей параметров:
        // 1. Блокировка учетной записи
        // 2. Включение/Выключение ограничений на выбираемые пароли
        // Регистрация новых пользователей
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly IMapper _mapper;
        private IMyPasswordValidatorService _passwordMyValidator;

        public AdministratorController(
            UserManager<MyIdentityUser> userManager, 
            IMapper mapper, 
            IMyPasswordValidatorService myPasswordValidator)
        {
            _userManager = userManager;
            _mapper = mapper;
            _passwordMyValidator = myPasswordValidator;

        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            SetLayOut(2);
            return View();
        }

        #region Register

        [HttpGet]
        public IActionResult RegisterUser()
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            SetLayOut();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterViewModel model)
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            SetLayOut();

            if (ModelState.IsValid)
            {
                var isEnable = await _userManager.FindByNameAsync(model.UserName);
                if(isEnable is not null)
                {
                    ModelState.AddModelError("", $"Пользователь с именем {model.UserName} уже существует!");
                    return View(model);
                }

                if (model.PasswordValidation)
                {
                    var passwordValid = _passwordMyValidator.Validate(model.Password!);
                    if (!passwordValid.Succeeded)
                    {
                        foreach (var error in passwordValid.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return View(model);
                    }
                }

                MyIdentityUser user = new MyIdentityUser
                {
                    UserName = model.UserName,
                    BlockedUser = model.BlockedUser,
                    PasswordValidation = model.PasswordValidation
                };

                var resultPassword = await _userManager.CreateAsync(user, model.Password);
                if (!resultPassword.Succeeded)
                {
                    foreach (var error in resultPassword.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }

                var resultRole = await _userManager.AddToRoleAsync(user, AppData.UserRoleName);
                if (!resultRole.Succeeded)
                {
                    foreach (var error in resultRole.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }

                SetLayOut(1);
                return RedirectToAction("AllUsers", "Administrator");
            }

            return View(model);
        }

        #endregion

        #region UpdateUser
        [HttpGet]
        public IActionResult AllUsers()
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            var allUsers = _userManager.Users.ToList();
            var Users = _mapper.Map<List<UpdateUserForAdminViewModel>>(allUsers);

            SetLayOut(1);
            return View(Users);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser(string userId)
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            SetLayOut();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                {
                    ModelState.AddModelError("", $"Пользователь c Id: { userId }  не найден!");
                    return View();
                }

                return View(_mapper.Map<UpdateUserForAdminViewModel>(user));
            }

            SetLayOut(1);
            return RedirectToAction("AllUsers", "Administrator");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UpdateUserForAdminViewModel modelUser)
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            SetLayOut();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(modelUser.Id);
                if(user == null)
                {
                    ModelState.AddModelError("", $"Пользователь { modelUser.UserName } (Id: { modelUser.Id } ) не найден!");
                    return View(modelUser);
                }

                if (await _userManager.IsInRoleAsync(user, AppData.AdministratorRoleName) && modelUser.BlockedUser)
                {
                    ModelState.AddModelError("", $"Учетную запись администратора заблокировать нельзя!");
                    return View(modelUser);
                }

                // изменение записи пользователя
                user.BlockedUser = modelUser.BlockedUser;
                user.PasswordValidation = modelUser.PasswordValidation;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(modelUser);
                }

                SetLayOut(1);
                return RedirectToAction("AllUsers", "Administrator");
            }

            return View(modelUser);
        }

        #endregion

        private void SetLayOut(int NavBar = 0)
        {
            ViewBag.Admin = true;
            ViewBag.NavBar = NavBar;
        }
    }
}
