namespace key_vault.Models
{
    public class Account
    {
        public int? AccountId { get; set; }
        public string Name { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
