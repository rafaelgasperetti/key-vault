using key_vault.Models.Interfaces;

namespace key_vault.Models
{
    public class APIEnvironment : IEnvironment
    {
        public string DatabaseHost { get; set; }
        public string DatabasePort { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
        public bool EncryptValues { get; set; }
        public string Secret { get; set; }
        public string JWTIssuer { get; set; }
        public string JWTAudience { get; set; }
        public Version Version { get; set; }
    }
}
