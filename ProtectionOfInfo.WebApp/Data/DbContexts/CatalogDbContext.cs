using Microsoft.EntityFrameworkCore;
using ProtectionOfInfo.WebApp.Data.Base;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Data.Configurations.Interfacess;
using System.Linq;

namespace ProtectionOfInfo.WebApp.Data
{
    public class CatalogDbContext : DbContextBase
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options) { }
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Publisher> Publishers { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(Startup).Assembly,
                x => x.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>) &&
                    i == typeof(ICatalogEntitiesConfigurations)));
            base.OnModelCreating(modelBuilder);
        }
    }
}
