using key_vault.Models;

namespace key_vault.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<Account> Get(int? accountId);
        
        public Task<Account> GetByName(string accountName);

        public Task<Account> Create(Account account);

        public Task Delete(int? accountId);
    }
}
