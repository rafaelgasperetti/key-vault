using key_vault.Models;

namespace key_vault.Services.Interfaces
{
    public interface IAccountService
    {
        public Account Get(int accountId, string apiKey);

        public Account GetByAccountId(int accountId);

        public Account Create(Account account);

        public void Delete(int accountId);
    }
}
