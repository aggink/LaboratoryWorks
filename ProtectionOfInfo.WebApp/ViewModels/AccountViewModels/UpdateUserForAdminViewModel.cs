using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;


namespace ProtectionOfInfo.WebApp.ViewModels.AccountViewModels
{
    public class UpdateUserForAdminViewModel
    {
        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = "Id пользователя")]
        public string? Id { get; set; }

        [ValidateNever]
        [Display(Name = "Имя пользователя")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = "Заблокирован пользователь")]
        public bool BlockedUser { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = "Валидация пароля")]
        public bool PasswordValidation { get; set; }

    }
}
