using key_vault.Database;
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
        private readonly TestEnvironment TestEnvironment;
        private readonly MySqlDb Database;
        private readonly IServiceProvider ServiceProvider;

        public BaseTest(bool initDb = true)
        {
            (TestEnvironment, ServiceProvider) = Initialize();

            if (initDb)
            {
                Database = new(TestEnvironment.ToAPIEnvironment());
                Database.BeginTransaction().Wait();
            }
        }

        private (TestEnvironment,IServiceProvider) Initialize()
        {
            var assembly = Assembly.GetAssembly(typeof(Initializer));
            string name = assembly.GetName().Name;
            string appSettingsDirectory = assembly.Location[..(assembly.Location.LastIndexOf(name) - Path.DirectorySeparatorChar.ToString().Length)];

            var builder = new ConfigurationBuilder()
                    .SetBasePath(appSettingsDirectory)
                    .AddJsonFile(Strings.BaseTest_Initialize_AppSettings)
                    .AddEnvironmentVariables();

            var config = builder.Build();
            var env = Initializer.GetTestEnvironment(config);

            IServiceCollection services = new ServiceCollection();
            services.Configure<TestEnvironment>(config.GetSection(nameof(TestEnvironment)));
            
            Initializer.Initialize(services, env);
            return new(env, services.BuildServiceProvider());
        }

        private void CleanUp()
        {
            using MySqlDb db = new(TestEnvironment.ToAPIEnvironment());

            db.CreateComand(Strings.BaseTest_CleanUpSecretKey).ExecuteNonQueryAsync().Wait();
            db.CreateComand(Strings.BaseTest_CleanUpAccount).ExecuteNonQueryAsync().Wait();
        }

        public TestEnvironment GetTestEnvironment()
        {
            return TestEnvironment;
        }

        public T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>();
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
