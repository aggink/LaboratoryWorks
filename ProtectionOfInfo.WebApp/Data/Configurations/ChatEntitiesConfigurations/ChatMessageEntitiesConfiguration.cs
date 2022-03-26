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
            builder.Property(x => x.Message);
            builder.Property(x => x.ContentType);
            builder.Property(x => x.Data);
            builder.Property(x => x.Extension);
            builder.Property(x => x.FileName);
        }

        protected override string TableName()
        {
            return "Messages";
        }
    }
}
