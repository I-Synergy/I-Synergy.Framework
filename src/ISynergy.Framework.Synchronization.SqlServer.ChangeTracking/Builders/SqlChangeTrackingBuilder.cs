using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.SqlServer.Builders;
using ISynergy.Framework.Synchronization.SqlServer.Utilities;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Builders
{
    public class SqlChangeTrackingBuilder : SqlBuilder
    {
        public override async Task EnsureDatabaseAsync(DbConnection connection, DbTransaction transaction = null)
        {
            // Chek if db exists
            await base.EnsureDatabaseAsync(connection, transaction).ConfigureAwait(false);

            // Check if we are using change tracking and it's enabled on the source
            var isChangeTrackingEnabled = await SqlManagementUtils.IsChangeTrackingEnabledAsync(connection as SqlConnection, transaction as SqlTransaction).ConfigureAwait(false);

            if (!isChangeTrackingEnabled)
                throw new MissingChangeTrackingException(connection.Database);

        }
    }
}
