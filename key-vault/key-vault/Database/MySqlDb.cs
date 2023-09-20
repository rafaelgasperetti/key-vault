using key_vault.Database.Interfaces;
using key_vault.Models;
using key_vault.Models.Interfaces;
using key_vault.Properties;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace key_vault.Database
{
    public class MySqlDb : IDatabase
    {
        private readonly IEnvironment Environment;
        private MySqlConnection Connection;
        private DbTransaction Transaction;

        public MySqlDb(IEnvironment env)
        {
            Environment = env;
            Connection = (MySqlConnection)OpenConnection().Result;
        }

        public MySqlDb(IEnvironment env, bool openConnection)
        {
            Environment = env;

            if (openConnection)
            {
                Connection = (MySqlConnection)OpenConnection().Result;
            }
        }

        public async Task<DbConnection> OpenConnection(bool mainDb = true)
        {
            string dbString = mainDb ? $"Database={Strings.DatabaseName};" : string.Empty;

            string connString = string.Format(Strings.ConnectionString, Environment.DatabaseHost, Environment.DatabasePort, Environment.DatabaseUser, Environment.DatabasePassword, dbString);
            var connection = new MySqlConnection(connString);
            await connection.OpenAsync();

            return connection;
        }

        public DbConnection GetConnection()
        {
            return Connection;
        }

        public DbCommand CreateComand(string query)
        {
            var cmd = Connection.CreateCommand();
            cmd.Connection = Connection;
            cmd.CommandText = query;
            return cmd;
        }

        public DbParameter GetParameter(string name, object value)
        {
            return new MySqlParameter(name, value);
        }

        public async Task<int?> GetLastId()
        {
            using var cmd = Connection.CreateCommand();
            cmd.Connection = Connection;
            cmd.CommandText = Strings.MySql_GetLastInsertedId;
            var result = await cmd.ExecuteScalarAsync();
            return result == null ? null : Convert.ToInt32(result.ToString());
        }

        public void Dispose()
        {
            try
            {
                Connection?.Dispose();
                Connection = null;
            }
            catch
            {

            }

            GC.SuppressFinalize(this);
        }

        public async Task BeginTransaction()
        {
            Transaction ??= await Connection.BeginTransactionAsync();
        }

        public async Task Commit()
        {
            await Transaction?.CommitAsync();
        }

        public async Task Rollback()
        {
            await Transaction?.RollbackAsync();
        }
    }
}
