using Microsoft.Extensions.DependencyInjection;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.InterfaceManager;
using System.Linq;
using System.Reflection;

namespace ProtectionOfInfo.WebApp.Infrastructure.Managers.Base
{
    public static class ManagerRegistration
    {
        public static void AddTransientManagers(IServiceCollection services)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract);
            foreach (var type in types)
            {
                foreach (var i in type.GetInterfaces())
                {

                    if (!i.IsGenericType || i.GetGenericTypeDefinition() != typeof(IMyManager<>)) continue;

                    var interfaceType = typeof(IMyManager<>).MakeGenericType(i.GetGenericArguments());
                    services.AddTransient(interfaceType, type);
                }
            }

            services.AddTransient<IBookManager, BookManager>();
        }
    }
}
