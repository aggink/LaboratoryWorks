using ProtectionOfInfo.WebApp.Data.Base.EntitiesBase;
using ProtectionOfInfo.WebApp.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectionOfInfo.WebApp.Data.CatalogEntities
{
    public class Book : Auditable
    {
        public string Name { get; set; } = null!;
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        public Guid AuthorId { get; set; }
        public Guid PublisherId { get; set; }
        public Guid CategoryId { get; set; }
        public Author? Author { get; set; }
        public Publisher? Publisher { get; set; }
        public Category? Category { get; set; }
    }
}
