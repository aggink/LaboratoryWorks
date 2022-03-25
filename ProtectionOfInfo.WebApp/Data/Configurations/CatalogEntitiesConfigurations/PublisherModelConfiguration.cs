using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Data.Configurations;
using ProtectionOfInfo.WebApp.Data.Configurations.Base;
using ProtectionOfInfo.WebApp.Data.Configurations.Interfacess;

namespace ProtectionOfInfo.WebApp.Data.CatalogEntitiesConfigurations
{
    public class PublisherModelConfiguration : AuditableModelConfigurationBase<Publisher>, ICatalogEntitiesConfigurations
    {
        protected override void AddBuilder(EntityTypeBuilder<Publisher> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(EntitiesModelsConfiguration.PublisherNameMaxLength).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(EntitiesModelsConfiguration.PublisherDescriptionMaxLength).IsRequired();

            builder.HasMany(x => x.Books);
        }

        protected override string TableName()
        {
            return "Publisher";
        }
    }
}
