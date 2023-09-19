using key_vault.Database.Interfaces;
using key_vault.Initializer.Jwt.Interfaces;
using key_vault.Models;
using key_vault.Properties;
using key_vault.Services.Interfaces;

namespace key_vault.Services
{
    public class AccountService : IAccountService
    {
        private readonly IDatabase Database;
        private readonly IEncryption Encryption;

        public AccountService(IDatabase db, IEncryption encryption)
        {
            Database = db;
            Encryption = encryption;
        }

        public Account Get(int accountId, string apiKey)
        {
            using var cmd = Database.CreateComand(Strings.AccountService_Get);
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.AccountId), accountId));
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.APIKey), apiKey));

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new Account()
            {
                AccountId = reader.GetInt32(0),
                Name = reader.GetString(1),
                APIKey = reader.GetGuid(2),
                CreatedAt = reader.GetDateTime(3),
                DeletedAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                Token = Encryption.GenerateToken(accountId, reader.GetGuid(2))
            };
        }

        public Account GetByAccountId(int accountId)
        {
            using var cmd = Database.CreateComand(Strings.AccountService_GetById);
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.AccountId), accountId));

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new Account()
            {
                AccountId = reader.GetInt32(0),
                Name = reader.GetString(1),
                APIKey = reader.GetGuid(2),
                CreatedAt = reader.GetDateTime(3),
                DeletedAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                Token = Encryption.GenerateToken(accountId, reader.GetGuid(2))
            };
        }

        public Account Create(Account account)
        {
            account.APIKey = Guid.NewGuid();

            using var cmd = Database.CreateComand(Strings.AccountService_Create);
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.Name), account.Name));
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.APIKey), account.APIKey));

            cmd.ExecuteNonQuery();
            var id = (int) Database.GetLastId();

            account = GetByAccountId(id);
            return account;
        }

        public void Delete(int accountId)
        {
            using var cmd = Database.CreateComand(Strings.AccountService_Delete);
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.AccountId), accountId));

            cmd.ExecuteNonQuery();
        }
    }
}
