using key_vault.Services.Interfaces;

namespace key_vault.Models
{
    public class SecretResponseAttributes
    {
        public string name { get; set; }
        public bool enabled { get; set; }
        public long created { get; set; }
        public long updated { get; set; }
        public string recoveryLevel { get; set; }
    }

    public class SecretResponse
    {
        public string name { get; set; }
        public string value { get; set; }
        public Uri id { get; set; }
        public SecretResponseAttributes attributes { get; set; }
    }
}
