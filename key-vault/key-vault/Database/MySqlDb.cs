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
        private static readonly SemaphoreSlim Semaphore = new(1);

        public MySqlDb(APIEnvironment env)
        {
            Environment = env;
            Connection = (MySqlConnection)OpenConnection().Result;
        }

        public MySqlDb(APIEnvironment env, bool openConnection)
        {
            Environment = env;

            if (openConnection)
            {
                Connection = (MySqlConnection)OpenConnection().Result;
            }
        }

        public async Task<DbConnection> OpenConnection(bool mainDb = true)
        {

            await Semaphore.WaitAsync();

            try
            {
                string dbString = mainDb ? string.Format(Strings.MySql_DatabaseNameString, Strings.DatabaseName) : string.Empty;

                string connString = string.Format(Strings.ConnectionString, Environment.DatabaseHost, Environment.DatabasePort, dbString);
                var connection = new MySqlConnection(connString);
                await connection.OpenAsync();

                return connection;
            }
            catch (Exception ex)
            {
                var message = string.Format(Strings.MySql_UnableToConnectMessage, Environment.DatabaseHost, Environment.DatabasePort, Environment.DatabaseUser, Environment.DatabasePassword, Enum.GetName(Environment.Environment));
                throw new Exception(message, ex);
            }
            finally
            {
                Semaphore.Release();
            }
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

        public bool HasTransaction()
        {
            return Transaction != null;
        }

        public async Task BeginTransaction()
        {
            Transaction ??= await Connection.BeginTransactionAsync();
        }

        public async Task Commit()
        {
            if (Environment.Environment == APIEnvironment.EnvironmentName.LocalApp || Environment.Environment == APIEnvironment.EnvironmentName.ProdApp)
            {
                await Transaction?.CommitAsync();
            }
        }

        public async Task Rollback()
        {
            await Transaction?.RollbackAsync();
        }
    }
}
