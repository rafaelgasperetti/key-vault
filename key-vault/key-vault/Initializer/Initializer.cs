using key_vault.Database;
using key_vault.Models;

namespace key_vault.Initializer
{
    public static class Initializer
    {
        public static APIEnvironment GetEnvironment(IConfiguration configuration)
        {
            var host = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.DatabaseHost)}"];
            var port = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.DatabasePort)}"];
            var user = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.DatabaseUser)}"];
            var password = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.DatabasePassword)}"];
            var encryptVales = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.EncryptValues)}"];
            var secret = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.Secret)}"];
            var jwtIssuer = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.JWTIssuer)}"];
            var jwtAudience = configuration[$"{nameof(APIEnvironment)}:{nameof(APIEnvironment.JWTAudience)}"];

            return new APIEnvironment()
            {
                DatabaseHost = host,
                DatabasePort = port,
                DatabaseUser = user,
                DatabasePassword = password,
                EncryptValues = !string.IsNullOrEmpty(encryptVales) && bool.Parse(encryptVales),
                Secret = secret,
                JWTIssuer = jwtIssuer,
                JWTAudience = jwtAudience
            };
        }

        public static void Migrate(APIEnvironment env)
        {
            using var db = new MySqlDb(env, false);
            using var migrator = new Migrator(db);
            migrator.Migrate();
        }
    }
}
