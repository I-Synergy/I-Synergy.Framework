using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Database;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SQLite.Builders
{
    public class SqliteBuilder : DbBuilder
    {
        public override Task EnsureDatabaseAsync(DbConnection connection, DbTransaction transaction = null)
            => Task.CompletedTask;

        public override Task<SyncTable> EnsureTableAsync(string tableName, string schemaName, DbConnection connection, DbTransaction transaction = null)
            => Task.FromResult(new SyncTable(tableName));

        public override Task<(string DatabaseName, string Version)> GetHelloAsync(DbConnection connection, DbTransaction transaction = null)
            => throw new NotImplementedException();
    }
}
