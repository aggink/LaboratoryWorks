using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using ProtectionOfInfo.WebApp.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Data
{
    /// <summary>
    /// Инициализация БД. Добавление учетной записи администратора.
    /// </summary>
    public static class MyIdentityUserInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            const string userName = "admin";
            const string userEmail = "admin@mail.ru";
            const string password = "123qwe123";

            var scope = serviceProvider.CreateScope();
            await using var context = scope.ServiceProvider.GetService<CatalogDbContext>();
            await using var contextMyKeys = scope.ServiceProvider.GetService<MyKeysContext>();
            //проверка на существование БД
            var isExists = context!.GetService<IDatabaseCreator>() is RelationalDatabaseCreator databaseCreator && await databaseCreator.ExistsAsync();
            if (isExists) return;

            var isExistsMyKeys = context!.GetService<IDatabaseCreator>() is RelationalDatabaseCreator databaseCreatorMyKey && await databaseCreatorMyKey.ExistsAsync();
            if (isExistsMyKeys) return;

            await context!.Database.MigrateAsync();
            await contextMyKeys!.Database.MigrateAsync();

            var userManager = scope.ServiceProvider.GetService<UserManager<MyIdentityUser>>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            var roles = AppData.Roles.ToArray();

            if(userManager is null || roleManager is null)
            {
                throw new Exception("UserManager or RoleManager not registered");
            }

            foreach(var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            if (await userManager.FindByNameAsync(userName) is not null) return;

            var user = new MyIdentityUser
            {
                UserName = userName,
                Email = userEmail
            };

            IdentityResult identityResult;
            identityResult = await userManager.CreateAsync(user, password);
            IdentityResultHandler(identityResult);

            identityResult = await userManager.AddToRolesAsync(user, roles);
            IdentityResultHandler(identityResult);

            await context.SaveChangesAsync();
            await contextMyKeys.SaveChangesAsync();
        }

        private static void IdentityResultHandler(IdentityResult result)
        {
            if (result.Succeeded is false)
            {
                var message = string.Join(", ", result.Errors.Select(x => $"{x.Code}: {x.Description}"));
                throw new Exception(message);
            }
        }
    }
}
