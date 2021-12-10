using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Model.Parsers;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Extensions;
using ISynergy.Framework.Synchronization.SqlServer.Triggers;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Triggers
{
    /// <summary>
    /// SQL change tracking builing trigger.
    /// </summary>
    public class SqlChangeTrackingBuilderTrigger : SqlBuilderTrigger
    {
        private ParserName _tableName;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="tableDescription"></param>
        /// <param name="tableName"></param>
        /// <param name="trackingName"></param>
        /// <param name="setup"></param>
        public SqlChangeTrackingBuilderTrigger(SyncTable tableDescription, ParserName tableName, ParserName trackingName, SyncSetup setup)
            : base(tableDescription, tableName, trackingName, setup)
        {
            _tableName = tableName;
        }

        public override Task<DbCommand> GetCreateTriggerCommandAsync(DbTriggerType triggerType, DbConnection connection, DbTransaction transaction)
                        => Task.FromResult<DbCommand>(null);

        public override Task<DbCommand> GetDropTriggerCommandAsync(DbTriggerType triggerType, DbConnection connection, DbTransaction transaction)
                        => Task.FromResult<DbCommand>(null);

        public override Task<DbCommand> GetExistsTriggerCommandAsync(DbTriggerType triggerType, DbConnection connection, DbTransaction transaction) =>
            connection.ToCommandAsync(transaction, _tableName);
    }
}
