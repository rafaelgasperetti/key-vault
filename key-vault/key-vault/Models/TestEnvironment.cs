using key_vault.Models.Interfaces;

namespace key_vault.Models
{
    public class TestEnvironment : IEnvironment
    {
        public string DatabaseHost { get; set; }
        public string DatabasePort { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
        public string KeyVaultAPIUrl { get; set; }

        public APIEnvironment ToAPIEnvironment()
        {
            return new APIEnvironment()
            {
                DatabaseHost = DatabaseHost,
                DatabasePort = DatabasePort,
                DatabaseUser = DatabaseUser,
                DatabasePassword = DatabasePassword,
                EncryptValues = false,
                Secret = null,
                JWTIssuer = null,
                JWTAudience = null,
                Version = null
            };
        }
    }
}
