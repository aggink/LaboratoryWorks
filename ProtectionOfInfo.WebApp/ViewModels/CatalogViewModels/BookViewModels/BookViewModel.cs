using ProtectionOfInfo.WebApp.ViewModels.Base;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.AuthorViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.CategoryViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.PublisherViewModels;

namespace ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.BookViewModels
{
    public class BookViewModel : ViewModelBase
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public AuthorViewModel? AuthorViewModel { get; set; }
        public CategoryViewModel? CategoryViewModel { get; set; }
        public PublisherViewModel? PublisherViewModel { get; set; }
    }
}
