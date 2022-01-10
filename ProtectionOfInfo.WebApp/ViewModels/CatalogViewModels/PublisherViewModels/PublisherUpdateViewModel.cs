using ProtectionOfInfo.WebApp.Data.Configurations;
using ProtectionOfInfo.WebApp.ViewModels.Base;
using System.ComponentModel.DataAnnotations;

namespace ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.PublisherViewModels
{
    public class PublisherUpdateViewModel : ViewModelBase
    {
        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [StringLength(EntitiesModelsConfiguration.PublisherNameMaxLength, ErrorMessage = ErrorMessageForAttributes.StringLengthErrorMessange)]
        [Display(Name = EntitiesModelsConfiguration.PublisherName)]
        public string? Name { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [StringLength(EntitiesModelsConfiguration.PublisherDescriptionMaxLength, ErrorMessage = ErrorMessageForAttributes.StringLengthErrorMessange)]
        [Display(Name = EntitiesModelsConfiguration.PublisherDescription)]
        public string? Description { get; set; }
    }
}
