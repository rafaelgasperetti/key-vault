using System.Data.Common;

namespace key_vault.Database.Interfaces
{
    public interface IDatabase : IDisposable
    {
        public Task<DbConnection> OpenConnection(bool mainDb = true);

        public DbConnection GetConnection();

        public DbCommand CreateComand(string query);

        public DbParameter GetParameter(string name, object value);

        public Task<int?> GetLastId();

        public Task BeginTransaction();

        public Task Commit();

        public Task Rollback();
    }
}
