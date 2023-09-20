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

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(Account))
            {
                return false;
            }

            var accObj = (Account)obj;

            return ((AccountId == null && accObj.AccountId == null) || AccountId == accObj.AccountId) &&
                Name.Equals(accObj.Name) &&
                ((TenantId == null && accObj.TenantId == null) || (TenantId?.Equals(accObj.TenantId) ?? false)) &&
                ((ClientId == null && accObj.ClientId == null) || (ClientId?.Equals(accObj.ClientId) ?? false)) &&
                ((ClientSecret == null && accObj.ClientSecret == null) || (ClientSecret?.Equals(accObj.ClientSecret) ?? false)) &&
                ((CreatedAt == null && accObj.CreatedAt == null) || (CreatedAt?.Equals(accObj.CreatedAt) ?? false)) &&
                ((DeletedAt == null && accObj.DeletedAt == null) || (DeletedAt?.Equals(accObj.DeletedAt) ?? false));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
