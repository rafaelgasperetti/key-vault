using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using key_vault_test.Properties;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using static key_vault.Models.APIEnvironment;

namespace key_vault_test.Vault
{
    public class Vault
    {
        private Uri BaseUri { get; set; }
        private string TenantID { get; set; }
        private string ClientID { get; set; }
        private string ClientSecret { get; set; }
        private EnvironmentName Environment { get; }

        public Vault(string baseUrl, string clientSecret, EnvironmentName environment)
        {
            BaseUri = string.IsNullOrEmpty(baseUrl) ? throw new InvalidDataException("Must fill baseUrl in Vault") : new Uri(baseUrl);
            TenantID = Guid.NewGuid().ToString();
            ClientID = Guid.NewGuid().ToString();
            ClientSecret = string.IsNullOrEmpty(clientSecret) ? throw new InvalidDataException("Must fill clientSecret in Vault") : clientSecret;
            Environment = environment;
        }

        private TokenCredential GetCredential()
        {
            return new ClientSecretCredential(TenantID, ClientID, ClientSecret);
        }

        private HttpClientTransport GetTransport(HttpContent? content = null)
        {
            HttpClientHandler? clientHandler = null;

            if (Environment != EnvironmentName.ProdApp)
            {
                clientHandler = new()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                };
            }

            var messageHandler = new VaultClientHandler(content, clientHandler);
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
            catch (Exception ex)
            {
                var message = string.Format(Strings.KeyVault_UnableToDoAction, BaseUri, TenantID, Enum.GetName(Environment));
                throw new RequestFailedException(message, ex);
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
            catch (Exception ex)
            {
                var message = string.Format(Strings.KeyVault_UnableToDoAction, BaseUri, TenantID, Enum.GetName(Environment));
                throw new RequestFailedException(message, ex);
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
            catch (Exception ex)
            {
                var message = string.Format(Strings.KeyVault_UnableToDoAction, BaseUri, TenantID, Enum.GetName(Environment));
                throw new RequestFailedException(message, ex);
            }
        }
    }
}
