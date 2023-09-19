namespace key_vault.Models
{
    public class Account
    {
        public int? AccountId { get; set; }
        public string Name { get; set; }
        public Guid? APIKey { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? Token { get; set; }
    }
}
