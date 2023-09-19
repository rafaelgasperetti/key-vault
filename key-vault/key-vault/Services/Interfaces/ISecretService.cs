using key_vault.Models;

namespace key_vault.Services.Interfaces
{
    public interface ISecretService
    {
        public SecretKey Get(int accountId, string name);

        public SecretKey Create(SecretKey secretKey);

        public void Delete(int accountId, string name);
    }
}
