﻿using key_vault.Database.Interfaces;
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

        public SecretService(APIEnvironment env, IDatabase db, IAccountService accountService, IEncryption encryption)
        {
            Environment = env;
            Database = db;
            AccountService = accountService;
            Encryption = encryption;
        }

        public SecretKey Get(int accountId, string name)
        {
            if (AccountService.Get(accountId) == null)
            {
                return null;
            }

            using var cmd = Database.CreateComand(Strings.SecretService_Get);
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.AccountId), accountId));
            cmd.Parameters.Add(Database.GetParameter(nameof(SecretKey.Name), name));
            
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

            return new SecretKey()
            {
                SecretKeyId = reader.GetInt32(0),
                AccountId = reader.GetInt32(1),
                Name = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Version = reader.GetInt32(4),
                Value = value,
                CreatedAt = reader.GetDateTime(6),
                DeletedAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            };
        }

        public SecretKey Create(SecretKey secretKey)
        {
            if (AccountService.Get(secretKey.AccountId) == null)
            {
                return null;
            }

            var dbSecretKey = Get(secretKey.AccountId, secretKey.Name);

            if (dbSecretKey == null)
            {
                secretKey.Version = 1;
            }
            else
            {
                secretKey.Version = dbSecretKey.Version + 1;
            }

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

            dbSecretKey = Get(secretKey.AccountId, secretKey.Name);

            secretKey.SecretKeyId = dbSecretKey.SecretKeyId;
            secretKey.CreatedAt = dbSecretKey.CreatedAt;
            secretKey.Value = dbSecretKey.Value;

            return secretKey;
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
