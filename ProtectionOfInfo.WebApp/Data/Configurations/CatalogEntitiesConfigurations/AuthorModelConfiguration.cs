using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Data.Configurations;
using ProtectionOfInfo.WebApp.Data.Configurations.Base;
using ProtectionOfInfo.WebApp.Data.Configurations.Interfacess;

namespace ProtectionOfInfo.WebApp.Data.CatalogEntitiesConfigurations
{
    public class AuthorModelConfiguration : AuditableModelConfigurationBase<Author>, ICatalogEntitiesConfigurations
    {
        protected override void AddBuilder(EntityTypeBuilder<Author> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(EntitiesModelsConfiguration.AuthorNameMaxLength).IsRequired();
            builder.Property(x => x.Biography).HasMaxLength(EntitiesModelsConfiguration.AuthorBiographyMaxLength).IsRequired();

            builder.HasMany(x => x.Books);
        }

        protected override string TableName()
        {
            return "Author";
        }
    }
}
