using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtectionOfInfo.WebApp.Data.Configurations.Interfaces;
using ProtectionOfInfo.WebApp.Data.Entities;

namespace ProtectionOfInfo.WebApp.Data.Configurations
{
    public class UserModelConfiguration : IEntityTypeConfiguration<MyIdentityUser>, IUserEntitiesConfigurations
    {
        public void Configure(EntityTypeBuilder<MyIdentityUser> builder)
        {
            builder.Property(x => x.FirstAccess).IsRequired();
            builder.Property(x => x.BlockedUser).IsRequired();
            builder.Property(x => x.PasswordValidation).IsRequired();
        }
    }
}
