using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Model.Parsers;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Adapters;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Adapters
{
    public class SqlChangeTrackingSyncAdapter : SqlSyncAdapter
    {
        public SqlChangeTrackingSyncAdapter(SyncTable tableDescription, ParserName tableName, ParserName trackingName, SyncSetup setup) : base(tableDescription, tableName, trackingName, setup)
        {
        }

        /// <summary>
        /// Overriding adapter since the update metadata is not a stored proc that we can override
        /// </summary>
        public override DbCommand GetCommand(DbCommandType nameType, SyncFilter filter)
        {
            if (nameType == DbCommandType.UpdateMetadata)
            {
                var c = new SqlCommand("Set @sync_row_count = 1;");
                c.Parameters.Add("@sync_row_count", SqlDbType.Int);
                return c;
            }

            return base.GetCommand(nameType, filter);
        }

    }
}
