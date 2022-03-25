using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtectionOfInfo.WebApp.Data.Configurations.Base;
using ProtectionOfInfo.WebApp.Data.Configurations.Interfaces;
using ProtectionOfInfo.WebApp.Data.Entities.ChatEntities;

namespace ProtectionOfInfo.WebApp.Data.Configurations.ChatEntitiesConfigurations
{
    public class FileDescriptionEntitiesConfiguration : AuditableModelConfigurationBase<FileDescription>, IChatEntitiesConfigurations
    {
        protected override void AddBuilder(EntityTypeBuilder<FileDescription> builder)
        {
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.ContentType).IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.Extension).IsRequired();
            builder.Property(x => x.FileName).IsRequired();
        }

        protected override string TableName()
        {
            return "Files";
        }
    }
}
