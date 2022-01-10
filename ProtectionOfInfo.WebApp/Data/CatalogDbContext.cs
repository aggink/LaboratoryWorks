using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProtectionOfInfo.WebApp.Data.Base;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;

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
    }

    public class MyKeysContext : DbContext, IDataProtectionKeyContext
    {
        // A recommended constructor overload when using EF Core 
        // with dependency injection.
        public MyKeysContext(DbContextOptions<MyKeysContext> options) : base(options) { }

        // This maps to the table that stores keys.
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
    }
}

//  Add-Migration CreateIdentitySchema -Context CatalogDbContext -OutputDir "Data/Migrations/CatalogDbContextMigrations"
//  Add-Migration AddDataProtectionKeys -Context MyKeysContext -OutputDir "Data/Migrations/MyKeysContextMigrations"
//  Update - database - Context CatalogDbContext
//  Update-database -Context MyKeysContext

