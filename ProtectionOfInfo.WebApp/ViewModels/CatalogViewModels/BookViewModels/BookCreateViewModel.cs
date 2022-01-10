using ProtectionOfInfo.WebApp.Data.Configurations;
using ProtectionOfInfo.WebApp.ViewModels.Base;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.AuthorViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.CategoryViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.PublisherViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.BookViewModels
{
    public class BookCreateViewModel : IBookParamViewModel, IViewModel
    {
        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [StringLength(EntitiesModelsConfiguration.BookNameMaxLength, ErrorMessage = ErrorMessageForAttributes.StringLengthErrorMessange)]
        [Display(Name = EntitiesModelsConfiguration.BookName)]
        public string? Name { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = EntitiesModelsConfiguration.BookPrice)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = EntitiesModelsConfiguration.BookAuthorId)]
        public Guid? AuthorId { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = EntitiesModelsConfiguration.BookPublisherId)]
        public Guid? PublisherId { get; set; }

        [Required(ErrorMessage = ErrorMessageForAttributes.RequiredErrorMessange)]
        [Display(Name = EntitiesModelsConfiguration.BookCategoryId)]
        public Guid? CategoryId { get; set; }

        public List<AuthorViewModel>? Authors { get; set; }
        public List<PublisherViewModel>? Publishers { get; set; }
        public List<CategoryViewModel>? Categories { get; set; }
    }
}
