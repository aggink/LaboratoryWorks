using System.ComponentModel.DataAnnotations;


namespace ProtectionOfInfo.WebApp.ViewModels.AccountViewModels
{
    public class UpdateUserViewModel
    {
        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = "Имя пользователя")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [DataType(DataType.Password)]
        [Display(Name = "Старый Пароль")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый Пароль")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтвердить новый пароль")]
        public string? PasswordConfirm { get; set; }
    }
}
