using key_vault.Database.Interfaces;
using key_vault.Models;
using key_vault.Properties;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace key_vault.Database
{
    public class MySqlDb : IDatabase
    {
        private readonly APIEnvironment Environment;
        private MySqlConnection Connection;
        private DbTransaction Transaction;

        public MySqlDb(APIEnvironment env)
        {
            Environment = env;
            Connection = (MySqlConnection)OpenConnection();
        }

        public MySqlDb(APIEnvironment env, bool openConnection)
        {
            Environment = env;

            if (openConnection)
            {
                Connection = (MySqlConnection)OpenConnection();
            }
        }

        public DbConnection OpenConnection(bool mainDb = true)
        {
            string dbString = mainDb ? $"Database={Strings.DatabaseName};" : string.Empty;

            string connString = string.Format(Strings.ConnectionString, Environment.DatabaseHost, Environment.DatabasePort, Environment.DatabaseUser, Environment.DatabasePassword, dbString);
            var connection = new MySqlConnection(connString);
            connection.Open();

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

        public int? GetLastId()
        {
            using var cmd = Connection.CreateCommand();
            cmd.Connection = Connection;
            cmd.CommandText = Strings.MySql_GetLastInsertedId;
            var result = cmd.ExecuteScalar();
            return result == null ? null : Convert.ToInt32(result.ToString());
        }

        public void Dispose()
        {
            Connection?.Dispose();
            Connection = null;

            GC.SuppressFinalize(this);
        }

        public void BeginTransaction()
        {
            Transaction ??= Connection.BeginTransaction();
        }

        public void Commit()
        {
            Transaction?.Commit();
        }
    }
}
