using Microsoft.EntityFrameworkCore;
using ProtectionOfInfo.WebApp.Data.Base;
using ProtectionOfInfo.WebApp.Data.Configurations.InterfacesConfigurations;
using ProtectionOfInfo.WebApp.Data.Entities.TelegramEntities;
using System.Linq;

namespace ProtectionOfInfo.WebApp.Data.DbContexts
{
    public class TelegramDbContext : DbContextBase
    {
        public TelegramDbContext(DbContextOptions<TelegramDbContext> options)
            : base(options) { }

        public DbSet<FileTelegram> File { get; set; } = null!;
        public DbSet<MessageTelegram> MessageTelegram { get; set; } = null!;
        public DbSet<UserTelegram> UserTelegram { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(Startup).Assembly,
                x => x.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>) &&
                    i == typeof(ITelegramEntitiesConfigurations)));
            base.OnModelCreating(modelBuilder);
        }
    }
}
