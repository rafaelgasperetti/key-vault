using Azure;
using key_vault.Database.Interfaces;
using key_vault.Helpers;
using key_vault.Models;
using key_vault_test.Properties;
using key_vault_test.Tests.Base;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace key_vault_test.Tests
{
    public class SecretAPITest : BaseTest
    {
        private readonly Account Account;
        private readonly Vault.Vault Vault;

        public SecretAPITest() : base(false)
        {
            Account = ConfigureAccount().Result;
            Vault = new Vault.Vault(GetEnvironment().KeyVaultAPIUrl.ToString(), Account.ClientSecret!);
        }

        protected override void CleanUp()
        {
            if (Account == null)
            {
                return;
            }

            var db = GetService<IDatabase>();
            using var cmd = db.CreateComand(Strings.BaseTest_CleanUpSecretKey);
            cmd.Parameters.Add(db.GetParameter(nameof(Account.AccountId), Account.AccountId));
            cmd.ExecuteNonQueryAsync().Wait();

            cmd.CommandText = Strings.BaseTest_CleanUpAccount;
            cmd.ExecuteNonQueryAsync().Wait();
        }

#if DEBUG
        [Fact]
#else
        [Fact(Skip = API_SKIP_REASON)]
#endif
        public async Task TestCrudSecret()
        {
            string secretName = Helper.RandomString();
            string secretValue = Helper.RandomString();

            await Vault.Create(secretName, secretValue);

            var actualResult = await Vault.Secret(secretName);
            Assert.Equal(secretValue, actualResult);

            secretValue = Helper.RandomString();
            await Vault.Create(secretName, secretValue);

            actualResult = await Vault.Secret(secretName);
            Assert.Equal(secretValue, actualResult);

            await Vault.Delete(secretName);
            await Assert.ThrowsAsync<RequestFailedException>(() => Vault.Delete(secretName));
        }

        private async Task<Account> ConfigureAccount()
        {
            using HttpClient client = new()
            {
                BaseAddress = GetEnvironment().KeyVaultAPIUrl
            };

            string accountName = Helper.RandomString();
            dynamic body = new
            {
                Name = accountName
            };
            var content = new StringContent(JsonConvert.SerializeObject(body), new MediaTypeHeaderValue("application/json"));

            var rawResponse = await client.PostAsync(Strings.AccountsEndpoint, content);
            Assert.Equal(HttpStatusCode.OK, rawResponse.StatusCode);

            var responseStr = await rawResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Account>(responseStr)!;
        }

        private async Task DeleteAccount()
        {
            using HttpClient client = new()
            {
                BaseAddress = GetEnvironment().KeyVaultAPIUrl
            };

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Account.ClientSecret}");

            var rawResponse = await client.DeleteAsync(Strings.AccountsEndpoint);
            Assert.Equal(HttpStatusCode.OK, rawResponse.StatusCode);
        }
    }
}