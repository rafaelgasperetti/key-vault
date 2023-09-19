namespace key_vault.Models
{
    public class SecretAttributes
    {
        public bool enabled { get; set; }
        public long created { get; set; }
        public long updated { get; set; }
        public string recoveryLevel { get; set; }
    }

    public class SecretResponse
    {
        public string value { get; set; }
        public Uri id { get; set; }
        public SecretAttributes attributes { get; set; }
    }
}
