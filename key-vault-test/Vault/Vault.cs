using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace key_vault_test.Vault
{
    public class Vault
    {
        private Uri BaseUri { get; set; }
        private string TenantID { get; set; }
        private string ClientID { get; set; }
        private string ClientSecret { get; set; }

        public Vault(string baseUrl, string clientSecret)
        {
            BaseUri = string.IsNullOrEmpty(baseUrl) ? throw new InvalidDataException("Must fill baseUrl in Vault") : new Uri(baseUrl);
            TenantID = Guid.NewGuid().ToString();
            ClientID = Guid.NewGuid().ToString();
            ClientSecret = string.IsNullOrEmpty(clientSecret) ? throw new InvalidDataException("Must fill clientSecret in Vault") : clientSecret;
        }

        private TokenCredential GetCredential()
        {
            return new ClientSecretCredential(TenantID, ClientID, ClientSecret);
        }

        private HttpClientTransport GetTransport(HttpContent? content = null)
        {
            var messageHandler = new VaultClientHandler(content);
            HttpClient client = new(messageHandler);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ClientSecret}");
            client.BaseAddress = BaseUri;

            var transport = new HttpClientTransport(client);
            return transport;
        }

        public async Task<string> Secret(string key)
        {
            try
            {
                var options = new SecretClientOptions
                {
                    Transport = GetTransport()
                };

                var client = new SecretClient(vaultUri: BaseUri, credential: GetCredential(), options);
                KeyVaultSecret secret = await client.GetSecretAsync(key);

                return secret.Value;
            }
            catch
            {
                throw;
            }
        }

        public async Task Create(string key, string secret)
        {
            try
            {
                dynamic body = new
                {
                    value = secret
                };
                var bodyContent = new StringContent(JsonConvert.SerializeObject(body), new MediaTypeHeaderValue("application/json"));

                var options = new SecretClientOptions()
                {
                    Transport = GetTransport(bodyContent)
                };

                var client = new SecretClient(vaultUri: BaseUri, credential: GetCredential(), options);
                var rawSecret = new KeyVaultSecret(key, secret);
                await client.SetSecretAsync(rawSecret);
            }
            catch
            {
                throw;
            }
        }

        public async Task Delete(string key)
        {
            try
            {
                var options = new SecretClientOptions()
                {
                    Transport = GetTransport()
                };

                var client = new SecretClient(vaultUri: BaseUri, credential: GetCredential(), options);

                var keyResult = await client.GetSecretAsync(key);

                if (keyResult != null && !(keyResult.Value.Properties.Enabled ?? false))
                {
                    return;
                }

                var operation = await client.StartDeleteSecretAsync(key);
                await operation.WaitForCompletionAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
