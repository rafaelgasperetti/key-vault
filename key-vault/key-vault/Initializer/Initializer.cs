using key_vault.Database;
using key_vault.Database.Interfaces;
using key_vault.Initializer.Jwt.Interfaces;
using key_vault.Initializer.Jwt;
using key_vault.Models;
using key_vault.Services.Interfaces;
using key_vault.Services;
using System.Reflection;
using key_vault.Helpers;
using key_vault.Properties;

namespace key_vault.Initializer
{
    public static class Initializer
    {
        private static bool Migrated = false;
        private static readonly SemaphoreSlim Semaphore = new(1);
        private const int MYSQL_WAIT_SECONDS = 15;

        public static APIEnvironment GetAPIEnvironment(IConfiguration configuration)
        {
            var assembly = Assembly.GetAssembly(typeof(Initializer));

            var envStr = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.Environment)}"];
            var host = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.DatabaseHost)}"];
            var port = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.DatabasePort)}"];
            var user = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.DatabaseUser)}"];
            var password = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.DatabasePassword)}"];
            var encryptVales = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.EncryptValues)}"];
            var secret = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.Secret)}"];
            var jwtIssuer = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.JWTIssuer)}"];
            var jwtAudience = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.JWTAudience)}"];
            var keyVaultAPIUrl = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.KeyVaultAPIUrl)}"];
            
            var version = assembly.GetName().Version;

            _ = Enum.TryParse(envStr, out APIEnvironment.EnvironmentName env);

            return new APIEnvironment()
            {
                Environment = env,
                DatabaseHost = host,
                DatabasePort = port,
                DatabaseUser = user,
                DatabasePassword = password,
                EncryptValues = !string.IsNullOrEmpty(encryptVales) && bool.Parse(encryptVales),
                Secret = secret,
                JWTIssuer = jwtIssuer,
                JWTAudience = jwtAudience,
                KeyVaultAPIUrl = new Uri(keyVaultAPIUrl),
                Version = version
            };
        }

        public static void Initialize(IServiceCollection services, APIEnvironment env)
        {
            Migrate(env);

            services.AddSingletonIfNotExists(env);
            services.AddScopedIfNotExists<IDatabase, MySqlDb>();
            services.AddScopedIfNotExists<IEncryption, Encryption>();
            services.AddScopedIfNotExists<IAccountService, AccountService>();
            services.AddScopedIfNotExists<ISecretService, SecretService>();

            services.AddHttpContextAccessor();
        }

        private static void Migrate(APIEnvironment env)
        {
            Semaphore.Wait();

            try
            {
                if (Migrated)
                {
                    return;
                }

                Migrated = true;

                if (env.Environment != APIEnvironment.EnvironmentName.ProdApp)
                {
                    Console.WriteLine(Strings.Initializer_MySqlWaitMessage);
                    Thread.Sleep(MYSQL_WAIT_SECONDS * 1000);
                }

                using var db = new MySqlDb(env, false);
                using var migrator = new Migrator(db);
                migrator.Migrate();
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
