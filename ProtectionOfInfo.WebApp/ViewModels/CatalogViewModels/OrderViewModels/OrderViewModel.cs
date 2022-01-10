using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.ViewModels.Base;
using ProtectionOfInfo.WebApp.ViewModels.UserViewModels;
using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.OrderViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        public UserViewModel? User { get; set; }
        public List<Book>? Books { get; set; }
        public decimal Price { get; set; }
    }
}
