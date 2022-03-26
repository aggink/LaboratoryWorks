using Microsoft.EntityFrameworkCore;
using ProtectionOfInfo.WebApp.Data.Base;
using ProtectionOfInfo.WebApp.Data.Configurations.Interfaces;
using ProtectionOfInfo.WebApp.Data.Entities.ChatEntities;
using System.Linq;

namespace ProtectionOfInfo.WebApp.Data
{
    public class ChatDbContext : DbContextBase
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options) { }

        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(Startup).Assembly,
                x => x.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>) &&
                    i == typeof(IChatEntitiesConfigurations)));

            base.OnModelCreating(modelBuilder);
        }
    }
}
