using key_vault.Database.Interfaces;
using key_vault.Helpers;
using key_vault.Models;
using key_vault.Services.Interfaces;
using key_vault_test.Properties;
using key_vault_test.Tests.Base;

namespace key_vault_test.Tests
{
    public class SecretServiceTest : BaseTest
    {
        [Fact]
        public async Task TestCreateNewSecretInvalidAccountId()
        {
            var service = GetService<ISecretService>();

            await Assert.ThrowsAsync<InvalidDataException>(() => service.Create(null, null, null));

            var accountName = Helper.RandomString();
            await Assert.ThrowsAsync<InvalidDataException>(() => service.Create(accountName, null, null));
        }

        [Fact]
        public async Task TestCreateNewSecretSuccess()
        {
            var accountService = GetService<IAccountService>();
            var secretService = GetService<ISecretService>();

            string accountName = Helper.RandomString();
            string accountValue = Helper.RandomString();
            string secretName = Helper.RandomString();

            Account account = await accountService.Create(new Account()
            {
                Name = accountName
            });

            var secret = await secretService.Create(account.Name, secretName, new SecretRequest()
            {
                name = secretName,
                value = accountValue
            });

            var actualSecret = await secretService.Get(account.Name, secretName, null);
            Assert.Equal(secret, actualSecret);
        }

        [Fact]
        public async Task TestGetSecretInvalidAccountId()
        {
            var service = GetService<ISecretService>();

            await Assert.ThrowsAsync<InvalidDataException>(() => service.Get(null, null, null));

            var accountName = Helper.RandomString();
            await Assert.ThrowsAsync<InvalidDataException>(() => service.Get(accountName, null, null));
        }

        [Fact]
        public async Task TestDeleteSecretInvalidAccountId()
        {
            var service = GetService<ISecretService>();

            await Assert.ThrowsAsync<InvalidDataException>(() => service.Delete(null, null));

            var accountName = Helper.RandomString();
            await Assert.ThrowsAsync<InvalidDataException>(() => service.Delete(accountName, null));
        }

        [Fact]
        public async Task TestDeleteSecretSuccess()
        {
            var accountService = GetService<IAccountService>();
            var secretService = GetService<ISecretService>();

            string accountName = Helper.RandomString();
            string secretName = Helper.RandomString();
            string secretValue = Helper.RandomString();

            Account account = await accountService.Create(new Account()
            {
                Name = accountName
            });

            var secret = await secretService.Create(account.Name, secretName, new SecretRequest()
            {
                value = secretValue
            });

            await secretService.Delete(account.Name, secretName);

            var db = GetService<IDatabase>();
            using var cmd = db.CreateComand(Strings.SecretTest_DeletedSecret);
            cmd.Parameters.Add(db.GetParameter(nameof(SecretResponse.name), secret.name));

            var deleted = await cmd.ExecuteScalarAsync();
            Assert.NotNull(deleted);
        }
    }
}
