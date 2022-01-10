using ProtectionOfInfo.WebApp.Data.Base.EntitiesBase;
using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.Data.CatalogEntities
{
    public class Category : Auditable
    {
        public string Name { get; set; } = null!;
        public string Synopsis { get; set; } = null!;
        public ICollection<Book>? Books { get; set; }
    }
}
