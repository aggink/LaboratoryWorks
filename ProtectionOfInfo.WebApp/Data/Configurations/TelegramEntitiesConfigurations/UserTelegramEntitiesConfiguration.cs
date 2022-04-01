using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtectionOfInfo.WebApp.Data.Configurations.Base;
using ProtectionOfInfo.WebApp.Data.Configurations.InterfacesConfigurations;
using ProtectionOfInfo.WebApp.Data.Entities.TelegramEntities;

namespace ProtectionOfInfo.WebApp.Data.Configurations.TelegramEntitiesConfigurations
{
    public class UserTelegramEntitiesConfiguration : AuditableModelConfigurationBase<UserTelegram>, ITelegramEntitiesConfigurations
    {
        protected override void AddBuilder(EntityTypeBuilder<UserTelegram> builder)
        {
            builder.Property(x => x.NickName).IsRequired();
            builder.Property(x => x.ChatId).IsRequired();

            builder.HasMany(x => x.Messages).WithOne(x => x.User);
        }

        protected override string TableName()
        {
            return "Users";
        }
    }
}
