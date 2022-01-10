using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.AuthorViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.CategoryViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.PublisherViewModels;
using System;
using System.Collections.Generic;


namespace ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.BookViewModels
{
    public interface IBookParamViewModel
    {
        Guid? AuthorId { get; set; }
        Guid? PublisherId { get; set; }
        Guid? CategoryId { get; set; }
        List<AuthorViewModel>? Authors { get; set; }
        List<PublisherViewModel>? Publishers { get; set; }
        List<CategoryViewModel>? Categories { get; set; }
    }
}
