using ProtectionOfInfo.WebApp.Data.Base.EntitiesBase;
using ProtectionOfInfo.WebApp.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectionOfInfo.WebApp.Data.CatalogEntities
{
    public class Order : Auditable
    {
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        public string Books { get; set; } = null!;
        public string UserId { get; set; } = null!;
    }
}
