using key_vault.Database.Interfaces;
using key_vault.Initializer.Jwt.Interfaces;
using key_vault.Models;
using key_vault.Properties;
using key_vault.Services.Interfaces;

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

        private SecretResponse GenerateSecretResponse(SecretKey secretKey)
        {
            if (secretKey == null)
            {
                return null;
            }

            var request = HttpContext.HttpContext.Request;
            string id = $"{request.Scheme}://{request.Host}/secrets/{secretKey.Name}/{secretKey.Version}";

            return new SecretResponse()
            {
                value = secretKey.Value,
                id = new Uri(id),
                attributes = new SecretAttributes()
                {
                    enabled = secretKey.DeletedAt == null,
                    created = secretKey.CreatedAt.Value.Ticks / TimeSpan.TicksPerMillisecond,
                    updated = secretKey.CreatedAt.Value.Ticks / TimeSpan.TicksPerMillisecond,
                    recoveryLevel = "Recoverable+Purgeable"
                }
            };
        }

        public SecretResponse Get(int accountId, string name, string? version)
        {
            if (AccountService.Get(accountId) == null)
            {
                return null;
            }

            using var cmd = Database.CreateComand(string.IsNullOrEmpty(version) ? Strings.SecretService_Get : Strings.SecretService_GetVersion);
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.AccountId), accountId));
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Name), name));

            if (!string.IsNullOrEmpty(version))
            {
                cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Version), version));
            }
            
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
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

        public SecretResponse Create(string name, SecretKey secretKey)
        {
            if (AccountService.Get(secretKey.AccountId) == null)
            {
                return null;
            }

            secretKey.Name = name;
            secretKey.Version = Guid.NewGuid();

            if (!string.IsNullOrEmpty(secretKey.Value) && Environment.EncryptValues)
            {
                secretKey.Value = Encryption.Encrypt(secretKey.Value);
            }

            using var cmd = Database.CreateComand(Strings.SecretService_Create);
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.AccountId), secretKey.AccountId));
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Name), secretKey.Name));
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Description), (object) secretKey.Description ?? DBNull.Value));
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Version), secretKey.Version));
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Value), (object) secretKey.Value ?? DBNull.Value));

            cmd.ExecuteNonQuery();

            return Get(secretKey.AccountId, secretKey.Name, null);
        }

        public void Delete(int accountId, string name)
        {
            if (AccountService.Get(accountId) == null)
            {
                return;
            }

            using var cmd = Database.CreateComand(Strings.SecretService_Delete);
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.AccountId), accountId));
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Name), name));

            cmd.ExecuteNonQuery();
        }
    }
}
