using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace key_vault.Helpers
{
    public static class EnvironmentHelper
    {
        public static string ReadResource(string name)
        {
            var dir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            var file = Path.Combine(dir, name);
            using StreamReader reader = new(file);
            return reader.ReadToEnd();
        }

        public static T ReadJsonResourceAs<T>(string name) where T : class
        {
            string value = ReadResource(name);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static IServiceCollection AddSingletonIfNotExists<TService>(this IServiceCollection services, TService implementationInstance) where TService : class
        {
            if (!services.Any(s => s.ServiceType == typeof(TService)))
            {
                return services.AddSingleton(implementationInstance);
            }
            else
            {
                return services;
            }
        }

        public static IServiceCollection AddScopedIfNotExists<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            if (!services.Any(s => s.ServiceType == typeof(TService)))
            {
                return services.AddScoped<TService, TImplementation>();
            }
            else
            {
                return services;
            }
        }
    }
}
