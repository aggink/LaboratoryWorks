using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Data.Configurations.Base;
using ProtectionOfInfo.WebApp.Data.Configurations.Interfacess;

namespace ProtectionOfInfo.WebApp.Data.CatalogEntitiesConfigurations
{
    public class OrderModelConfiguration : AuditableModelConfigurationBase<Order>, ICatalogEntitiesConfigurations
    {
        protected override void AddBuilder(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.Books).IsRequired();
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
        }

        protected override string TableName()
        {
            return "Order";
        }
    }
}
