using key_vault.Models;

namespace key_vault.Services.Interfaces
{
    public interface ISecretService
    {
        public const string DEFAULT_RECOVERY_LEVEL = "Recoverable+Purgeable";

        public Task<SecretResponse> Get(int? accountId, string name, string? version);

        public Task<SecretResponse> Create(int? accountId, string name, SecretRequest request);

        public Task<SecretResponse> Delete(int? accountId, string name);
    }
}
