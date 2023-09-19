using System.Data.Common;

namespace key_vault.Database.Interfaces
{
    public interface IDatabase : IDisposable
    {
        public DbConnection OpenConnection(bool mainDb = true);

        public DbConnection GetConnection();

        public DbCommand CreateComand(string query);

        public DbParameter GetParameter(string name, object value);

        public int? GetLastId();

        public void BeginTransaction();

        public void Commit();
    }
}
