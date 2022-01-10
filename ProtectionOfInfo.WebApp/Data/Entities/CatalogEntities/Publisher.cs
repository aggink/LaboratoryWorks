using ProtectionOfInfo.WebApp.Data.Base.EntitiesBase;
using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.Data.CatalogEntities
{
    public class Publisher : Auditable
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ICollection<Book>? Books {get; set;}
    }
}
