using ProtectionOfInfo.WebApp.Data.Base.EntitiesBase;
using ProtectionOfInfo.WebApp.Data.Entities;
using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.Data.CatalogEntities
{
    public class Author : Auditable
    {
        public string Name { get; set; } = null!;
        public string Biography { get; set; } = null!;
        public ICollection<Book>? Books { get; set; }
    }
}
