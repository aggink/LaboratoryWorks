using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace ProtectionOfInfo.WebApp.Infrastructure.Repositories.Base
{
    public static class RepositoryRegistration
    {
        public static void AddScopedRepositories(IServiceCollection services)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract);
            foreach(var type in types)
            {
                foreach (var i in type.GetInterfaces())
                {
                    if (!i.IsGenericType || i.GetGenericTypeDefinition() != typeof(IMyRepository<>)) continue;

                    var interfaceType = typeof(IMyRepository<>).MakeGenericType(i.GetGenericArguments());
                    services.AddScoped(interfaceType, type);
                }
            }

            //var repositories = Assembly.GetExecutingAssembly()
            //    .GetTypes()
            //    .Where(t => t.Name.EndsWith("Repository"))
            //    .ToList();
            //
            //if (!repositories.Any()) return;
            //
            //foreach (var _repository in repositories)
            //{
            //    services.AddScoped(_repository);
            //}
        }
    }
}
