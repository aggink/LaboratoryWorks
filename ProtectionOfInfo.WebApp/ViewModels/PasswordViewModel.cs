using System.ComponentModel.DataAnnotations;

namespace ProtectionOfInfo.WebApp.ViewModels
{
    public class PasswordViewModel
    {
        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }
    }
}
