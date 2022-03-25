using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Data.Configurations;
using ProtectionOfInfo.WebApp.Data.Configurations.Base;
using ProtectionOfInfo.WebApp.Data.Configurations.Interfacess;

namespace ProtectionOfInfo.WebApp.Data.CatalogEntitiesConfigurations
{
    public class BookModelConfiguration : AuditableModelConfigurationBase<Book>, ICatalogEntitiesConfigurations
    {
        protected override void AddBuilder(EntityTypeBuilder<Book> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(EntitiesModelsConfiguration.BookNameMaxLength).IsRequired();
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.CategoryId).IsRequired();
            builder.Property(x => x.PublisherId).IsRequired();
            builder.Property(x => x.AuthorId).IsRequired();

            builder.HasOne(x => x.Author);
            builder.HasOne(x => x.Publisher);
            builder.HasOne(x => x.Category);
        }

        protected override string TableName()
        {
            return "Book";
        }
    }
}
