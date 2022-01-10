using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtectionOfInfo.WebApp.Data;

namespace ProtectionOfInfo.WebApp.Controllers.User
{
    [Authorize(Roles = AppData.UserRoleName)]
    [AutoValidateAntiforgeryToken]
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated is false)
            {
                return View("Error", "Срок ожидания пользователя истек!");
            }

            SetLayOut();
            return View();
        }
        private void SetLayOut()
        {
            ViewBag.User = true;
        }
    }
}
