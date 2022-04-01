using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtectionOfInfo.WebApp.Data.Configurations.Base;
using ProtectionOfInfo.WebApp.Data.Configurations.InterfacesConfigurations;
using ProtectionOfInfo.WebApp.Data.Entities.TelegramEntities;

namespace ProtectionOfInfo.WebApp.Data.Configurations.TelegramEntitiesConfigurations
{
    public class FileTelegramEntitiesConfiguration : AuditableModelConfigurationBase<FileTelegram>, ITelegramEntitiesConfigurations
    {
        protected override void AddBuilder(EntityTypeBuilder<FileTelegram> builder)
        {
            builder.Property(x => x.ContentType).IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Extension).IsRequired();
            builder.Property(x => x.FileName).IsRequired();
            builder.Property(x => x.IsPublication).IsRequired();
            builder.Property(x => x.MessageId);
            builder.Property(x => x.Value);

            builder.HasOne(x => x.Message).WithMany(x => x.Files);
        }

        protected override string TableName()
        {
            return "Files";
        }
    }
}
