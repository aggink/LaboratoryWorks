using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtectionOfInfo.WebApp.Data.Configurations.Base;
using ProtectionOfInfo.WebApp.Data.Configurations.Interfaces;
using ProtectionOfInfo.WebApp.Data.Entities.ChatEntities;

namespace ProtectionOfInfo.WebApp.Data.Configurations.ChatEntitiesConfigurations
{
    public class ChatMessageEntitiesConfiguration : AuditableModelConfigurationBase<ChatMessage>, IChatEntitiesConfigurations
    {
        protected override void AddBuilder(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.FileId);
            builder.Property(x => x.Message);

            builder.HasOne(x => x.File);
        }

        protected override string TableName()
        {
            return "Messages";
        }
    }
}
