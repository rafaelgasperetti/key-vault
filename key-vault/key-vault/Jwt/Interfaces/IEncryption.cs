namespace key_vault.Initializer.Jwt.Interfaces
{
    public interface IEncryption
	{
		public string GenerateToken(int accountId, Guid tenantId, Guid clientId);

        public bool IsTokenValid(string token);

        public string Encrypt(string value);

        public string Decrypt(string value);

    }
}
