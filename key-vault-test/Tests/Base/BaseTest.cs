using key_vault.Database.Interfaces;
using key_vault.Initializer;
using key_vault.Models;
using key_vault_test.Properties;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace key_vault_test.Tests.Base
{
    public class BaseTest : IDisposable
    {
        protected const string API_SKIP_REASON = "Skipped due to be an API call into the tested project. Only possible locally.";

        private readonly APIEnvironment Environment;
        private readonly IDatabase Database;
        private readonly IServiceProvider ServiceProvider;

        public BaseTest(bool initDb = true)
        {
            (Environment, ServiceProvider) = Initialize();

            if (initDb)
            {
                Database = GetService<IDatabase>();
                Database.BeginTransaction().Wait();
            }
        }

        private (APIEnvironment, IServiceProvider) Initialize()
        {
            var assembly = Assembly.GetAssembly(typeof(Initializer));
            string name = assembly.GetName().Name;
            string appSettingsDirectory = assembly.Location[..(assembly.Location.LastIndexOf(name) - Path.DirectorySeparatorChar.ToString().Length)];

            var builder = new ConfigurationBuilder()
                    .SetBasePath(appSettingsDirectory)
                    .AddJsonFile(Strings.BaseTest_Initialize_AppSettings)
                    .AddEnvironmentVariables();

            var config = builder.Build();
            
            var env = Initializer.GetAPIEnvironment(config);
            env.Version = assembly.GetName().Version;

            IServiceCollection services = new ServiceCollection();
            services.Configure<APIEnvironment>(config.GetSection(nameof(Environment)));

            Initializer.Initialize(services, env);
            return new(env, services.BuildServiceProvider());
        }

        protected virtual void CleanUp()
        {

        }

        public T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>();
        }

        public APIEnvironment GetEnvironment()
        {
            return Environment;
        }

        public void Dispose()
        {
            try
            {
                if (Database != null)
                {
                    Database.Rollback().Wait();
                    Database.Dispose();
                }
                else
                {
                    CleanUp();
                }
            }
            catch
            {

            }

            GC.SuppressFinalize(this);
        }
    }
}
