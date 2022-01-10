using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.BookViewModels
{
    public class BooksViewModel
    {
        public List<BookViewModel>? Books { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPrice { get; set; }
    }
}
