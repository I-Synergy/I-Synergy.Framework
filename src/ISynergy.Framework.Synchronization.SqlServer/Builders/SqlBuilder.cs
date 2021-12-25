using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.SqlServer.Utilities;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Builders
{
    public class SqlBuilder : DbBuilder
    {
        public override async Task EnsureDatabaseAsync(DbConnection connection, DbTransaction transaction = null)
        {
            // Chek if db exists
            var exists = await SqlManagementUtils.DatabaseExistsAsync(connection as SqlConnection, transaction as SqlTransaction).ConfigureAwait(false);

            if (!exists)
                throw new MissingDatabaseException(connection.Database);
        }

        public override Task<SyncTable> EnsureTableAsync(string tableName, string schemaName, DbConnection connection, DbTransaction transaction = null)
            => Task.FromResult(new SyncTable(tableName, schemaName));

        public override async Task<(string DatabaseName, string Version)> GetHelloAsync(DbConnection connection, DbTransaction transaction = null)
        {
            return await SqlManagementUtils.GetHelloAsync(connection as SqlConnection, transaction as SqlTransaction).ConfigureAwait(false);

        }
    }
}
