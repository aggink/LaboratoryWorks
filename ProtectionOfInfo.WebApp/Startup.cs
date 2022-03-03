using Calabonga.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.Entities;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.Base;
using ProtectionOfInfo.WebApp.Infrastructure.Providers.Base;
using ProtectionOfInfo.WebApp.Infrastructure.Repositories.Base;
using ProtectionOfInfo.WebApp.Infrastructure.Services.ConvertToExcelService;
using ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService;
using ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService.Interface;
using ProtectionOfInfo.WebApp.Infrastructure.Services.DictionaryApiService;
using ProtectionOfInfo.WebApp.Infrastructure.Services.PasswordValidatorsService;
using ProtectionOfInfo.WebApp.Infrastructure.Services.PortInfoService;
using System;

namespace ProtectionOfInfo.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CatalogDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            // Add a DbContext to store your Database Keys
            services.AddDbContext<MyKeysContext>(options => 
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("UserDbConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddUnitOfWork<CatalogDbContext>();
            services.AddUnitOfWork<UserDbContext>();

            services.AddIdentity<MyIdentityUser, IdentityRole>(options =>
            {
                // требуется ли для входа подтвержденная учетная запись
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
                .AddEntityFrameworkStores<CatalogDbContext>();

            services.Configure<IdentityOptions>(config =>
            {
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireLowercase = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Home/LogIn";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.LoginPath = "/Home/LogIn";
                options.LogoutPath = "/Home/Index";
                options.SlidingExpiration = true;
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = ".MyApp.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(3600);
                options.Cookie.IsEssential = true;
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddRouting(config =>
            {
                config.LowercaseQueryStrings = true;
                config.LowercaseUrls = true;
            });

            // dependency injection

            services.AddTransient<IMyPasswordValidatorService, PasswordValidatorService>();
            
            // ---
            services.AddTransient<IDataEncryptionService, DataEncryptionService>();
            services.AddTransient<IMyUserRepository, UserRepository>();
            services.AddTransient<IDictionaryApi, DictionaryApi>();
            
            /// // aggink: update summary - 01.03.2022 1:18:51
            services.AddTransient<ICryptographyService, CryptographyService>();
            services.AddTransient<IPortInfoService, PortInfoService>();
            services.AddTransient<IConvertToExcel, ConvertToExcel>();
            // ---

            RepositoryRegistration.AddScopedRepositories(services);
            ProviderRegistration.AddTransientProviders(services);
            ManagerRegistration.AddTransientManagers(services);

            // other settings

            services.AddAntiforgery();
            services.AddDataProtection().PersistKeysToDbContext<MyKeysContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Dictionary",
                    pattern: "{controller=Dictionary}/{action=Index}/{word}/{code}");

                endpoints.MapControllerRoute(
                    name: "Sort",
                    pattern: "{controller}/{action}/{ObjectSort}/{TypeSort}");

                endpoints.MapControllerRoute(
                    name: "Search",
                    pattern: "{controller}/{action}/{ObjectSearch}/{Search}");

                endpoints.MapControllerRoute(
                    name: "admin",
                    pattern: "{controller=Administrator}/{action=UpdateUser}/{userId}");

                endpoints.MapControllerRoute(
                    name: "authors",
                    pattern: "{controller=Author}/{action=Update}/{Id}");

                endpoints.MapControllerRoute(
                    name: "category",
                    pattern: "{ controller=Category}/{action=Update}/{Id}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");

                endpoints.MapRazorPages();
            });
        }
    }
}
