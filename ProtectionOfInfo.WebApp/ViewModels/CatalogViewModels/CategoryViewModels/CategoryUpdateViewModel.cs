using ProtectionOfInfo.WebApp.Data.Configurations;
using ProtectionOfInfo.WebApp.ViewModels.Base;
using System.ComponentModel.DataAnnotations;

namespace ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.CategoryViewModels
{
    public class CategoryUpdateViewModel : ViewModelBase
    {
        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [StringLength(EntitiesModelsConfiguration.CategoryNameMaxLength, ErrorMessage = ErrorMessageForAttributes.StringLengthErrorMessange)]
        [Display(Name = EntitiesModelsConfiguration.CategoryName)]
        public string? Name { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [StringLength(EntitiesModelsConfiguration.CategorySynopsisMaxLength, ErrorMessage = ErrorMessageForAttributes.StringLengthErrorMessange)]
        [Display(Name = EntitiesModelsConfiguration.CategorySynopsis)]
        public string? Synopsis { get; set; }
    }
}
