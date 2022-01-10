using System.ComponentModel.DataAnnotations;

namespace ProtectionOfInfo.WebApp.ViewModels.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = "Имя пользователя")]
        public string? UserName { get; set; }


        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтвердить пароль")]
        public string? PasswordConfirm { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = "Заблокирован пользователь")]
        public bool BlockedUser { get; set; } = false;

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = "Валидация пароля")]
        public bool PasswordValidation { get; set; } = false;
    }
}
