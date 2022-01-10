using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Data.Configurations.Base;

namespace ProtectionOfInfo.WebApp.Data.CatalogEntitiesConfigurations
{
    public class OrderModelConfiguration : AuditableModelConfigurationBase<Order>
    {
        protected override void AddBuilder(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.Books).IsRequired();
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.UserId).IsRequired();

            builder.HasOne(x => x.User);
        }

        protected override string TableName()
        {
            return "Order";
        }
    }
}
