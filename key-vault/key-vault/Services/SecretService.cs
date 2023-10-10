using key_vault.Database.Interfaces;
using key_vault.Initializer.Jwt.Interfaces;
using key_vault.Models;
using key_vault.Properties;
using key_vault.Services.Interfaces;
using key_vault.Helpers;

namespace key_vault.Services
{
    public class SecretService : ISecretService
    {
        private readonly APIEnvironment Environment;
        private readonly IDatabase Database;
        private readonly IAccountService AccountService;
        private readonly IEncryption Encryption;
        private readonly IHttpContextAccessor HttpContext;

        public SecretService(APIEnvironment env, IDatabase db, IAccountService accountService, IEncryption encryption, IHttpContextAccessor httpContext)
        {
            Environment = env;
            Database = db;
            AccountService = accountService;
            Encryption = encryption;
            HttpContext = httpContext;
        }

        private SecretResponse GenerateSecretResponse(SecretKey secretKey, string recoveryLevel = ISecretService.DEFAULT_RECOVERY_LEVEL)
        {
            if (secretKey == null)
            {
                return new();
            }

            string id;

            if (Environment.Environment == APIEnvironment.EnvironmentName.LocalTest || Environment.Environment == APIEnvironment.EnvironmentName.ProdTest)
            {
                id = $"{Environment.KeyVaultAPIUrl}secrets/{secretKey.Name}/{secretKey.Version}";
            }
            else
            {
                var request = HttpContext.HttpContext.Request;
                id = $"{request.Scheme}://{request.Host}/secrets/{secretKey.Name}/{secretKey.Version}";
            }

            return new SecretResponse()
            {
                name = secretKey.Name,
                value = secretKey.Value,
                id = new Uri(id),
                attributes = new SecretResponseAttributes()
                {
                    enabled = secretKey.DeletedAt == null,
                    created = secretKey.CreatedAt.Value.GetCurrentTimestamp(),
                    updated = secretKey.CreatedAt.Value.GetCurrentTimestamp(),
                    recoveryLevel = recoveryLevel
                }
            };
        }

        public async Task<SecretResponse> Get(string? accountName, string name, string? version)
        {
            var account = await AccountService.GetByName(accountName) ?? throw new InvalidDataException(string.Format(Strings.Service_AccountNotFound, name, accountName));

            using var cmd = Database.CreateComand(string.IsNullOrEmpty(version) ? Strings.SecretService_Get : Strings.SecretService_GetVersion);
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.AccountId), account.AccountId));
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Name), name));

            if (!string.IsNullOrEmpty(version))
            {
                cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Version), version));
            }
            
            using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.Read())
            {
                throw new InvalidDataException(string.Format(Strings.Service_SecretNotFound, name, accountName));
            }

            string value = reader.IsDBNull(5) ? null : reader.GetString(5);

            if (!string.IsNullOrEmpty(value) && Environment.EncryptValues)
            {
                value = Encryption.Decrypt(value);
            }

            var secretKey = new SecretKey()
            {
                SecretKeyId = reader.GetInt32(0),
                AccountId = reader.GetInt32(1),
                Name = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Version = reader.GetGuid(4),
                Value = value,
                CreatedAt = reader.GetDateTime(6),
                DeletedAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            };

            return GenerateSecretResponse(secretKey);
        }

        public async Task<SecretResponse> Create(string? accountName, string name, SecretRequest request)
        {
            try
            {
                var account = (string.IsNullOrEmpty(accountName)  ? null : await AccountService.GetByName(accountName)) ?? throw new InvalidDataException(string.Format(Strings.Service_AccountNotFound, accountName));

                SecretKey secretKey = new()
                {
                    AccountId = account.AccountId.Value,
                    Name = name,
                    Version = Guid.NewGuid(),
                    Value = request.value
                };

                if (!string.IsNullOrEmpty(secretKey.Value) && Environment.EncryptValues)
                {
                    secretKey.Value = Encryption.Encrypt(secretKey.Value);
                }

                using var cmd = Database.CreateComand(Strings.SecretService_Create);
                cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.AccountId), secretKey.AccountId));
                cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Name), secretKey.Name));
                cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Description), (object)secretKey.Description ?? DBNull.Value));
                cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Version), secretKey.Version));
                cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Value), (object)secretKey.Value ?? DBNull.Value));

                await cmd.ExecuteNonQueryAsync();

                return await Get(accountName, secretKey.Name, null);
            }
            catch
            {
                throw;
            }
        }

        public async Task<SecretResponse> Delete(string? accountName, string name)
        {
            var account = (string.IsNullOrEmpty(accountName) ? null : await AccountService.GetByName(accountName)) ?? throw new InvalidDataException(string.Format(Strings.Service_AccountNotFound, accountName));

            var dbSecret = await Get(accountName, name, null);

            using var cmd = Database.CreateComand(Strings.SecretService_Delete);
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.AccountId), account.AccountId.Value));
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Name), name));

            await cmd.ExecuteNonQueryAsync();
            return dbSecret;
        }
    }
}
