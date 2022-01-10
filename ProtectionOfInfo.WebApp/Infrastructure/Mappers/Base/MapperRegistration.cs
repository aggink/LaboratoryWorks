using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProtectionOfInfo.WebApp.Infrastructure.Mappers.Base
{
    /// <summary>
    /// Регистрация всех классов используют AutoMapper
    /// </summary>
    public static class MapperRegistration
    {
        public static MapperConfiguration GetMapperConfiguration()
        {
            var profiles = GetProfiles();
            return new MapperConfiguration(config =>
            {
                // Activator.CreateInstance - Создает экземпляр указанного типа
                foreach (var profile in profiles.Select(profile => (Profile)Activator.CreateInstance(profile)!))
                {
                    config.AddProfile(profile);
                }
            });
        }

        private static List<Type> GetProfiles()
        {
            //получаем все типы классов, которые реализуют интерфейс IAutoMapper и не являются абстрактными
            return (from t in typeof(Startup).GetTypeInfo().Assembly.GetTypes()
                    where typeof(IAutoMapper).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract
                    select t).ToList();
        }
    }
}
