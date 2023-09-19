using key_vault.Models;

namespace key_vault.Services.Interfaces
{
    public interface ISecretService
    {
        public SecretResponse Get(int accountId, string name, string? version);

        public SecretResponse Create(string name, SecretKey secretKey);

        public void Delete(int accountId, string name);
    }
}
