using ProtectionOfInfo.WebApp.Data.Configurations;
using ProtectionOfInfo.WebApp.ViewModels.Base;
using System.ComponentModel.DataAnnotations;

namespace ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.AuthorViewModels
{
    public class AuthorCreateViewModel : IViewModel
    {
        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [StringLength(EntitiesModelsConfiguration.AuthorNameMaxLength, ErrorMessage =ErrorMessageForAttributes.StringLengthErrorMessange)]
        [Display(Name=EntitiesModelsConfiguration.AuthorName)]
        public string? Name { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [StringLength(EntitiesModelsConfiguration.AuthorBiographyMaxLength, ErrorMessage = ErrorMessageForAttributes.StringLengthErrorMessange)]
        [Display(Name=EntitiesModelsConfiguration.AuthorBiography)]
        public string? Biography { get; set; }

    }
}
