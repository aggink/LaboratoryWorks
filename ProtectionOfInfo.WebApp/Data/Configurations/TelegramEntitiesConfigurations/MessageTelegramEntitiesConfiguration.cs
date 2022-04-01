using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtectionOfInfo.WebApp.Data.Configurations.Base;
using ProtectionOfInfo.WebApp.Data.Configurations.InterfacesConfigurations;
using ProtectionOfInfo.WebApp.Data.Entities.TelegramEntities;

namespace ProtectionOfInfo.WebApp.Data.Configurations.TelegramEntitiesConfigurations
{
    public class MessageTelegramEntitiesConfiguration : AuditableModelConfigurationBase<MessageTelegram>, ITelegramEntitiesConfigurations
    {
        protected override void AddBuilder(EntityTypeBuilder<MessageTelegram> builder)
        {
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.MessageFromUser);
            builder.Property(x => x.MessageFromBot);

            builder.HasOne(x => x.User).WithMany(x => x.Messages);
            builder.HasMany(x => x.Files).WithOne(x => x.Message);
        }

        protected override string TableName()
        {
            return "Messages";
        }
    }
}
