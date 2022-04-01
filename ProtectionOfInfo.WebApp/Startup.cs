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
using ProtectionOfInfo.WebApp.Data.DbContexts;
using ProtectionOfInfo.WebApp.Data.Entities;
using ProtectionOfInfo.WebApp.Hubs;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.Base;
using ProtectionOfInfo.WebApp.Infrastructure.Providers.Base;
using ProtectionOfInfo.WebApp.Infrastructure.Repositories.Base;
using ProtectionOfInfo.WebApp.Infrastructure.Services.ConvertToExcelService;
using ProtectionOfInfo.WebApp.Infrastructure.Services.ConvertToWordService;
using ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService;
using ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService.Interface;
using ProtectionOfInfo.WebApp.Infrastructure.Services.CurrencyService;
using ProtectionOfInfo.WebApp.Infrastructure.Services.DictionaryApiService;
using ProtectionOfInfo.WebApp.Infrastructure.Services.PasswordValidatorsService;
using ProtectionOfInfo.WebApp.Infrastructure.Services.PortInfoService;
using ProtectionOfInfo.WebApp.TelegramBot;
using ProtectionOfInfo.WebApp.TelegramBot.Interfaces;
using ProtectionOfInfo.WebApp.TelegramBot.Managers;
using System;
using Telegram.Bot;

namespace ProtectionOfInfo.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            BotConfig = Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }

        public IConfiguration Configuration { get; }
        public BotConfiguration BotConfig { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ChatDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("ChatDbConnection")));

            services.AddDbContext<CatalogDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("CatalogDbConnection")));

            services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("UserDbConnection")));

            services.AddDbContext<MyKeysDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("MyKeyDbConnection")));

            services.AddDbContext<TelegramDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("TelegramDbConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddUnitOfWork<CatalogDbContext>();
            services.AddUnitOfWork<ChatDbContext>();
            services.AddUnitOfWork<TelegramDbContext>();

            services.AddIdentity<MyIdentityUser, IdentityRole>(options =>
            {
                // требуется ли для входа подтвержденная учетная запись
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
                .AddEntityFrameworkStores<UserDbContext>();

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

            // The Telegram.Bot library heavily depends on Newtonsoft.Json library to deserialize
            // incoming webhook updates and send serialized responses back.
            // Read more about adding Newtonsoft.Json to ASP.NET Core pipeline:
            //   https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-5.0#add-newtonsoftjson-based-json-format-support

            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddRazorPages();

            services.AddRouting(config =>
            {
                config.LowercaseQueryStrings = true;
                config.LowercaseUrls = true;
            });

            services.AddSignalR();

            // dependency injection

            services.AddTransient<IMyPasswordValidatorService, PasswordValidatorService>();

            services.AddTransient<IDataEncryptionService, DataEncryptionService>();
            services.AddTransient<IDictionaryApi, DictionaryApi>();
            
            services.AddTransient<ICryptographyService, CryptographyService>();
            services.AddTransient<IPortInfoService, PortInfoService>();
            services.AddTransient<IConvertToExcel, ConvertToExcel>();
            services.AddTransient<ISettingEDSFileService, SettingEDSFileService>();
            services.AddTransient<IConvertToWord, ConvertToWord>();
            services.AddTransient<IChatMessagesManager, ChatMessagesManager>();
            services.AddSingleton<ChatManager>();
            services.AddTransient<IFileTelegramManager, FileTelegramManager>();
            services.AddTransient<ICurrencyService, CurrencyService>();

            RepositoryRegistration.AddScopedRepositories(services);
            ProviderRegistration.AddTransientProviders(services);
            ManagerRegistration.AddTransientManagers(services);

            // other settings
            services.AddAntiforgery();
            services.AddDataProtection().PersistKeysToDbContext<MyKeysDbContext>();
            services.AddHostedService<CurrencyService>();
            services.AddMemoryCache();

            // Register named HttpClient to get benefits of IHttpClientFactory
            // and consume it with ITelegramBotClient typed client.
            // More read:
            //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
            //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient("TelegramBot")
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(BotConfig.BotToken, httpClient));
            
            // There are several strategies for completing asynchronous tasks during startup.
            // Some of them could be found in this article https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
            // We are going to use IHostedService to add and later remove Webhook
            //services.AddScoped<ITelegramBotClient, TelegramBotClient>();
            services.AddHostedService<WebhookConfigure>();
            
            services.AddScoped<IHandlerUpdateTelegramService, HandlerUpdateTelegramService>();
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
                    pattern: "{controller=Category}/{action=Update}/{id}");

                endpoints.MapControllerRoute(
                    name: "chat",
                    pattern: "{controller=Chat}/{action}/{id?}");

                endpoints.MapControllerRoute(
                    name: "chatfile",
                    pattern: "{controller=Chat}/{action=SendFileAsync}");

                endpoints.MapControllerRoute(
                    name: "TelegramBot",
                    pattern: "{controller=TelegramBot}/{action}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");


                // Configure custom endpoint per Telegram API recommendations:
                // https://core.telegram.org/bots/api#setwebhook
                // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
                // using a secret path in the URL, e.g. https://www.example.com/<token>.
                // Since nobody else knows your bot's token, you can be pretty sure it's us.
                var token = BotConfig.BotToken;
                endpoints.MapControllerRoute(name: "TelegramBot",
                                             pattern: $"bot/{token}",
                                             new { controller = "TelegramBot", action = "Post" });
                
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapHub<CommunicationHub>("/message");
            });
        }
    }
}
