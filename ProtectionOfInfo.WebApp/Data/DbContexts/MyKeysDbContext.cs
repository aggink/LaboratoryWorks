using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProtectionOfInfo.WebApp.Data
{
    public class MyKeysDbContext : DbContext, IDataProtectionKeyContext
    {
        // A recommended constructor overload when using EF Core 
        // with dependency injection.
        public MyKeysDbContext(DbContextOptions<MyKeysDbContext> options) : base(options) { }

        // This maps to the table that stores keys.
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
    }
}

