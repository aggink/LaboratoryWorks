using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Data.Configurations;
using ProtectionOfInfo.WebApp.Data.Configurations.Base;

namespace ProtectionOfInfo.WebApp.Data.CatalogEntitiesConfigurations
{
    public class CategoryModelConfiguration : AuditableModelConfigurationBase<Category>
    {
        protected override void AddBuilder(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(EntitiesModelsConfiguration.CategoryNameMaxLength).IsRequired();
            builder.Property(x => x.Synopsis).HasMaxLength(EntitiesModelsConfiguration.CategorySynopsisMaxLength).IsRequired();

            builder.HasMany(x => x.Books);
        }

        protected override string TableName()
        {
            return "Category";
        }
    }
}
