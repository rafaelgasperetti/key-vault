namespace key_vault.Models
{
    public class APIEnvironment
    {
        public enum EnvironmentName
        {
            LocalTest,
            ProdTest,
            LocalApp,
            ProdApp
        }

        public EnvironmentName Environment { get; set; }
        public string DatabaseHost { get; set; }
        public string DatabasePort { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
        public bool EncryptValues { get; set; }
        public string Secret { get; set; }
        public string JWTIssuer { get; set; }
        public string JWTAudience { get; set; }
        public Uri KeyVaultAPIUrl { get; set; }
        public Version Version { get; set; }
    }
}
