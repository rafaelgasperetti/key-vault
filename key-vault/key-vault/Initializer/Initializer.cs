using key_vault.Database;
using key_vault.Database.Interfaces;
using key_vault.Initializer.Jwt.Interfaces;
using key_vault.Initializer.Jwt;
using key_vault.Models;
using key_vault.Services.Interfaces;
using key_vault.Services;
using System.Reflection;
using key_vault.Models.Interfaces;
using key_vault.Helpers;

namespace key_vault.Initializer
{
    public static class Initializer
    {
        private static readonly object MigrationLock = new();
        private static bool Migrated = false;

        public static APIEnvironment GetAPIEnvironment(IConfiguration configuration)
        {
            var assembly = Assembly.GetAssembly(typeof(Initializer));

            var host = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.DatabaseHost)}"];
            var port = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.DatabasePort)}"];
            var user = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.DatabaseUser)}"];
            var password = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.DatabasePassword)}"];
            var encryptVales = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.EncryptValues)}"];
            var secret = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.Secret)}"];
            var jwtIssuer = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.JWTIssuer)}"];
            var jwtAudience = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.JWTAudience)}"];
            
            var version = assembly.GetName().Version;

            return new APIEnvironment()
            {
                DatabaseHost = host,
                DatabasePort = port,
                DatabaseUser = user,
                DatabasePassword = password,
                EncryptValues = !string.IsNullOrEmpty(encryptVales) && bool.Parse(encryptVales),
                Secret = secret,
                JWTIssuer = jwtIssuer,
                JWTAudience = jwtAudience,
                Version = version
            };
        }

        public static TestEnvironment GetTestEnvironment(IConfiguration configuration)
        {
            var host = configuration[$"{nameof(TestEnvironment)}:{nameof(TestEnvironment.DatabaseHost)}"];
            var port = configuration[$"{nameof(TestEnvironment)}:{nameof(TestEnvironment.DatabasePort)}"];
            var user = configuration[$"{nameof(TestEnvironment)}:{nameof(TestEnvironment.DatabaseUser)}"];
            var password = configuration[$"{nameof(TestEnvironment)}:{nameof(TestEnvironment.DatabasePassword)}"];
            var keyVaultAPIUrl = configuration[$"{nameof(TestEnvironment)}:{nameof(TestEnvironment.KeyVaultAPIUrl)}"];

            return new TestEnvironment()
            {
                DatabaseHost = host,
                DatabasePort = port,
                DatabaseUser = user,
                DatabasePassword = password,
                KeyVaultAPIUrl = keyVaultAPIUrl
            };
        }

        public static void Initialize(IServiceCollection services, IEnvironment env)
        {
            Migrate(env);

            services.AddSingletonIfNotExists(env);
            services.AddScopedIfNotExists<IDatabase, MySqlDb>();
            services.AddScopedIfNotExists<IEncryption, Encryption>();
            services.AddScopedIfNotExists<IAccountService, AccountService>();
            services.AddScopedIfNotExists<ISecretService, SecretService>();
        }

        private static void Migrate(IEnvironment env)
        {
            if (Migrated)
            {
                return;
            }

            Migrated = true;

            lock (MigrationLock)
            {
                using var db = new MySqlDb(env, false);
                using var migrator = new Migrator(db);
                migrator.Migrate();
            }
        }
    }
}
