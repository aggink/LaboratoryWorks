using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace ProtectionOfInfo.WebApp.Infrastructure.Providers.Base
{
    public static class ProviderRegistration
    {
        public static void AddTransientProviders(IServiceCollection services)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract);
            foreach (var type in types)
            {
                foreach (var i in type.GetInterfaces())
                {
                    if (!i.IsGenericType || i.GetGenericTypeDefinition() != typeof(IMyProvider<>)) continue;

                    var interfaceType = typeof(IMyProvider<>).MakeGenericType(i.GetGenericArguments());
                    services.AddTransient(interfaceType, type);
                }
            }

            //var providers = Assembly.GetExecutingAssembly()
            //    .GetTypes()
            //    .Where(t => t.Name.EndsWith("Provider"))
            //    .ToList();
            //
            //if (!providers.Any()) return;
            //
            //foreach (var _provider in providers)
            //{
            //    services.AddScoped(_provider);
            //}
        }
    }
}
