using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using ProtectionOfInfo.WebApp.Data.DbContexts;
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
            //база данный пользователей входит в catalogDbContext
            await using var catalogDbContext = scope.ServiceProvider.GetService<CatalogDbContext>();
            await using var userDbContext = scope.ServiceProvider.GetService<UserDbContext>();
            await using var myKeysDbContext = scope.ServiceProvider.GetService<MyKeysDbContext>();
            await using var chatDbContext = scope.ServiceProvider.GetService<ChatDbContext>();

            //проверка на существование БД

            var user_IsExists = userDbContext!.GetService<IDatabaseCreator>() is RelationalDatabaseCreator userDbCreator && await userDbCreator.ExistsAsync();
            if (!user_IsExists)
            {
                await userDbContext!.Database.MigrateAsync();
            }

            var myKeys_IsExists = myKeysDbContext!.GetService<IDatabaseCreator>() is RelationalDatabaseCreator myKeyDbCreator && await myKeyDbCreator.ExistsAsync();
            if (!myKeys_IsExists)
            {
                await myKeysDbContext!.Database.MigrateAsync();
            }

            var catalog_IsExists = catalogDbContext!.GetService<IDatabaseCreator>() is RelationalDatabaseCreator catalogDbCreator && await catalogDbCreator.ExistsAsync();
            if (!catalog_IsExists)
            {
                await catalogDbContext!.Database.MigrateAsync();
                await catalogDbContext!.SaveChangesAsync();
            }

            var chat_IsExists = chatDbContext!.GetService<IDatabaseCreator>() is RelationalDatabaseCreator chatDbCreator && await chatDbCreator.ExistsAsync();
            if (!chat_IsExists)
            {
                await chatDbContext!.Database.MigrateAsync();
                await chatDbContext!.SaveChangesAsync();
            }

            if (user_IsExists) return;

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

            await userDbContext!.SaveChangesAsync();
            await myKeysDbContext!.SaveChangesAsync();
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
