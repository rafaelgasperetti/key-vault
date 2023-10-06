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
    public class AccountAPITest : BaseTest
    {
        private int? AccountIdToDelete = null;

        public AccountAPITest() : base(false)
        {

        }

        protected override void CleanUp()
        {
            if (AccountIdToDelete == null)
            {
                return;
            }

            var db = GetService<IDatabase>();
            using var cmd = db.CreateComand(Strings.BaseTest_CleanUpAccount);
            cmd.Parameters.Add(db.GetParameter(nameof(Account.AccountId), AccountIdToDelete));
            cmd.ExecuteNonQueryAsync().Wait();
        }

        [Fact]
        public async Task TestCrudNewAccountSuccess()
        {
            string accountsEndpoint = "accounts";

            HttpClientHandler clientHandler = new()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };

            using HttpClient client = new(clientHandler)
            {
                BaseAddress = GetEnvironment().KeyVaultAPIUrl
            };

            string accountName = Helper.RandomString();
            dynamic body = new
            {
                Name = accountName
            };
            var content = new StringContent(JsonConvert.SerializeObject(body), new MediaTypeHeaderValue("application/json"));

            var rawResponse = await client.PostAsync(accountsEndpoint, content);
            var createResponseStr = await rawResponse.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, rawResponse.StatusCode);
            Assert.NotNull(createResponseStr);

            var createResponse = JsonConvert.DeserializeObject<Account>(createResponseStr);

            Assert.NotNull(createResponse);
            Assert.NotNull(createResponse.AccountId);
            Assert.Equal(accountName, createResponse.Name);
            Assert.NotNull(createResponse.TenantId);
            Assert.NotNull(createResponse.ClientId);
            Assert.NotNull(createResponse.ClientSecret);
            Assert.NotNull(createResponse.CreatedAt);

            AccountIdToDelete = createResponse.AccountId;

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {createResponse.ClientSecret}");

            rawResponse = await client.GetAsync(accountsEndpoint);
            var getResponseStr = await rawResponse.Content.ReadAsStringAsync();

            Assert.True(createResponseStr == getResponseStr, $"Expected:\n{createResponseStr}\nActual:\n{getResponseStr}");
            Assert.Equal(HttpStatusCode.OK, rawResponse.StatusCode);

            var getResponse = JsonConvert.DeserializeObject<Account>(getResponseStr);

            Assert.NotNull(getResponse);
            Assert.Equal(createResponse.AccountId, getResponse.AccountId);
            Assert.Equal(createResponse.Name, getResponse.Name);
            Assert.Equal(createResponse.TenantId, getResponse.TenantId);
            Assert.Equal(createResponse.ClientId, getResponse.ClientId);
            Assert.Equal(createResponse.ClientSecret, getResponse.ClientSecret);
            Assert.Equal(createResponse.CreatedAt, getResponse.CreatedAt);

            rawResponse = await client.DeleteAsync(accountsEndpoint);
            Assert.Equal(HttpStatusCode.OK, rawResponse.StatusCode);
        }
    }
}