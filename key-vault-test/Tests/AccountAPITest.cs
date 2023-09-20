using key_vault.Helpers;
using key_vault.Models;
using key_vault_test.Tests.Base;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace key_vault_test.Tests
{
    public class AccountAPITest : BaseTest
    {
        public AccountAPITest() : base(false)
        {

        }

        [Fact]
        public async Task TestCrudNewAccountSuccess()
        {
            string accountsEndpoint = "accounts";
            var uri = new Uri(GetTestEnvironment().KeyVaultAPIUrl);

            using HttpClient client = new()
            {
                BaseAddress = uri
            };

            string accountName = Helper.RandomString();
            dynamic body = new
            {
                Name = accountName
            };
            var content = new StringContent(JsonConvert.SerializeObject(body), new MediaTypeHeaderValue("application/json"));

            var rawResponse = await client.PostAsync(accountsEndpoint, content);
            var responseStr = await rawResponse.Content.ReadAsStringAsync();

            var createResponse = JsonConvert.DeserializeObject<Account>(responseStr);

            Assert.Equal(HttpStatusCode.OK, rawResponse.StatusCode);
            Assert.NotNull(createResponse);
            Assert.NotNull(createResponse.AccountId);
            Assert.Equal(accountName, createResponse.Name);
            Assert.NotNull(createResponse.TenantId);
            Assert.NotNull(createResponse.ClientId);
            Assert.NotNull(createResponse.ClientSecret);
            Assert.NotNull(createResponse.CreatedAt);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {createResponse.ClientSecret}");

            rawResponse = await client.GetAsync(accountsEndpoint);
            responseStr = await rawResponse.Content.ReadAsStringAsync();

            var getResponse = JsonConvert.DeserializeObject<Account>(responseStr);

            Assert.Equal(HttpStatusCode.OK, rawResponse.StatusCode);
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