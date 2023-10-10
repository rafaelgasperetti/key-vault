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
            var account = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.Account)}"];
            var accountToken = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.AccountToken)}"];
            
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
                Version = version,
                Account = account,
                AccountToken = accountToken
            };
        }

        public static void Initialize(IServiceCollection services, APIEnvironment env)
        {
            IEncryption encryption = new Encryption(env);

            services.AddSingletonIfNotExists(env);
            services.AddSingletonIfNotExists(encryption);
            services.AddScopedIfNotExists<IDatabase, MySqlDb>();
            services.AddScopedIfNotExists<IAccountService, AccountService>();
            services.AddScopedIfNotExists<ISecretService, SecretService>();

            services.AddHttpContextAccessor();

            Migrate(env, encryption);
        }

        private static void Migrate(APIEnvironment env, IEncryption encryption)
        {
            Semaphore.Wait();

            try
            {
                if (Migrated)
                {
                    return;
                }

                Migrated = true;
                CheckAndWaitDatabaseOnline(env);

                using var db = new MySqlDb(env, false);
                using var migrator = new Migrator(db);
                migrator.Migrate();

                if (!string.IsNullOrEmpty(env.Account) && !string.IsNullOrEmpty(env.AccountToken) && encryption.IsTokenValid(env.AccountToken))
                {
                    AccountService service = new(db, encryption);

                    if (service.GetByName(env.Account).Result == null)
                    {
                        service.Create(new Account()
                        {
                            Name = env.Account,
                            ClientSecret = env.AccountToken
                        }).Wait();

                        Console.WriteLine(string.Format("Created account \"{0}\"", env.Account));
                    }
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }

        private static void CheckAndWaitDatabaseOnline(APIEnvironment env)
        {
            if (env.Environment == APIEnvironment.EnvironmentName.ProdApp)
            {
                return;
            }

            var checkProgram = "mysqladmin";
            var checkCommand = $"ping -h {env.DatabaseHost} -P {env.DatabasePort} -u {env.DatabaseUser} -p{env.DatabasePassword}";

            bool success = false;
            int currentTryCount = 0;
            int maxTries = 5;
            string lastErrorMessage;

            while (!success && currentTryCount < maxTries)
            {
                try
                {
                    var result = CmdHelper.Execute(checkProgram, checkCommand);
                    success = result.Success && result.Message?.ToLower() == "mysqld is alive";
                    Console.WriteLine(result.Message);

                    if (success)
                    {
                        lastErrorMessage = null;
                    }
                    else
                    {
                        lastErrorMessage = result.Message;
                    }
                }
                catch (Exception ex)
                {
                    lastErrorMessage = ex.Message + (string.IsNullOrEmpty(ex.InnerException?.Message) ? string.Empty : (Environment.NewLine + ex.InnerException?.Message));
                }

                if (!string.IsNullOrEmpty(lastErrorMessage))
                {
                    currentTryCount++;
                    Console.WriteLine(string.Format(Strings.MySqlWaitingMessage, env.DatabaseHost, env.DatabasePort, currentTryCount, maxTries, lastErrorMessage));
                    Thread.Sleep(2000);
                }
            }
        }
    }
}
