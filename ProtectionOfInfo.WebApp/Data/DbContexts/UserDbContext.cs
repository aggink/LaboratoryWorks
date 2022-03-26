using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProtectionOfInfo.WebApp.Data.Configurations.Interfaces;
using ProtectionOfInfo.WebApp.Data.Entities;
using System.Linq;

namespace ProtectionOfInfo.WebApp.Data.DbContexts
{
    public class UserDbContext : IdentityDbContext<MyIdentityUser>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(Startup).Assembly,
                x => x.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>) &&
                    i == typeof(IUserEntitiesConfigurations)));

            base.OnModelCreating(modelBuilder);
        }
    }
}
