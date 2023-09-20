using key_vault.Services.Interfaces;

namespace key_vault.Models
{
    public class SecretRequestAttributes
    {
        public bool enabled { get; set; } = true;
        public long created { get; set; } = 0;
        public long updated { get; set; } = 0;
        public int exp { get; set; } = 0;
        public int nbf { get; set; } = 0;
        public int recoverableDays { get; set; } = 0;
        public string recoveryLevel { get; set; } = ISecretService.DEFAULT_RECOVERY_LEVEL;
    }

    public class SecretRequest
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public SecretRequestAttributes attributes { get; set; } = new();
        public string value { get; set; }
        public string? contentType { get; set; }
        public object? tags { get; set; }
    }
}
