using key_vault.Database.Interfaces;
using key_vault.Helpers;
using key_vault.Models;
using key_vault.Services.Interfaces;
using key_vault_test.Properties;
using key_vault_test.Tests.Base;

namespace key_vault_test.Tests
{
    public class AccountServiceTest : BaseTest
    {
        [Fact]
        public async Task TestCreateNewAccountInvalidAccountId()
        {
            var service = GetService<IAccountService>();

            await Assert.ThrowsAsync<InvalidDataException>(() => service.Get(null));

            int accountId = Helper.RandomInt(1, 1000);
            await Assert.ThrowsAsync<InvalidDataException>(() => service.Get(accountId));
        }

        [Fact]
        public async Task TestCreateNewAccountSuccess()
        {
            var service = GetService<IAccountService>();

            string accountName = Helper.RandomString();

            Account account = await service.Create(new Account()
            {
                Name = accountName
            });
            Assert.Equal(accountName, account.Name);

            Account actualAccount = await service.Get(account.AccountId);
            Assert.Equal(account, actualAccount);
        }

        [Fact]
        public async Task TestGetAccountByNameSuccess()
        {
            var service = GetService<IAccountService>();

            string accountName = Helper.RandomString();

            Account account = await service.Create(new Account()
            {
                Name = accountName
            });
            Assert.Equal(accountName, account.Name);

            Account actualAccount = await service.GetByName(account.Name);
            Assert.Equal(account, actualAccount);
        }

        [Fact]
        public async Task TestDeleteAccountSuccess()
        {
            var service = GetService<IAccountService>();

            string accountName = Helper.RandomString();

            Account account = await service.Create(new Account()
            {
                Name = accountName
            });
            Assert.Equal(accountName, account.Name);

            await service.Delete(account.AccountId);

            var db = GetService<IDatabase>();
            using var cmd = db.CreateComand(Strings.AccountTest_DeletedAccount);
            cmd.Parameters.Add(db.GetParameter(nameof(Account.AccountId), account.AccountId));

            var deleted = await cmd.ExecuteScalarAsync();
            Assert.NotNull(deleted);
        }
    }
}
