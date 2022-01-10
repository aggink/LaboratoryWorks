using ProtectionOfInfo.WebApp.Data.Configurations;
using ProtectionOfInfo.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.OrderViewModels
{
    public class OrderUpdateViewModel : ViewModelBase
    {
        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = EntitiesModelsConfiguration.OrderUserId)]
        public Guid? UserId { get; set; }
        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = EntitiesModelsConfiguration.OrderBooksId)]
        public List<Guid>? BooksId { get; set; }
        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = EntitiesModelsConfiguration.OrderPrice)]
        public decimal Price { get; set; }
    }
}
