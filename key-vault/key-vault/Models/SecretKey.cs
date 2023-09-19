namespace key_vault.Models
{
    public class SecretKey
    {
        public int? SecretKeyId { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? Version { get; set; }
        public string? Value { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
