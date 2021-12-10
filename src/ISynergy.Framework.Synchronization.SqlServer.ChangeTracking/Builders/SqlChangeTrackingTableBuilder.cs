using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Model.Parsers;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Builders;
using ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Procedures;
using ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Tables;
using ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Triggers;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Builders
{
    /// <summary>
    /// SQL change tracking table builder.
    /// </summary>
    public class SqlChangeTrackingTableBuilder : SqlTableBuilder
    {
        private SqlChangeTrackingBuilderTrackingTable _sqlChangeTrackingBuilderTrackingTable;
        private SqlChangeTrackingBuilderProcedure _sqlChangeTrackingBuilderProcedure;
        private SqlChangeTrackingBuilderTrigger _sqlChangeTrackingBuilderTrigger;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="tableDescription"></param>
        /// <param name="tableName"></param>
        /// <param name="trackingTableName"></param>
        /// <param name="setup"></param>
        public SqlChangeTrackingTableBuilder(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup)
            : base(tableDescription, tableName, trackingTableName, setup)
        {
            _sqlChangeTrackingBuilderTrackingTable = new SqlChangeTrackingBuilderTrackingTable(TableDescription, TableName, TrackingTableName, Setup);
            _sqlChangeTrackingBuilderProcedure = new SqlChangeTrackingBuilderProcedure(TableDescription, TableName, TrackingTableName, Setup);
            _sqlChangeTrackingBuilderTrigger = new SqlChangeTrackingBuilderTrigger(TableDescription, TableName, TrackingTableName, Setup);
        }

        public override Task<DbCommand> GetExistsStoredProcedureCommandAsync(DbStoredProcedureType storedProcedureType, SyncFilter filter, DbConnection connection, DbTransaction transaction)
            => _sqlChangeTrackingBuilderProcedure.GetExistsStoredProcedureCommandAsync(storedProcedureType, filter, connection, transaction);
        public override Task<DbCommand> GetCreateStoredProcedureCommandAsync(DbStoredProcedureType storedProcedureType, SyncFilter filter, DbConnection connection, DbTransaction transaction)
            => _sqlChangeTrackingBuilderProcedure.GetCreateStoredProcedureCommandAsync(storedProcedureType, filter, connection, transaction);
        public override Task<DbCommand> GetDropStoredProcedureCommandAsync(DbStoredProcedureType storedProcedureType, SyncFilter filter, DbConnection connection, DbTransaction transaction)
            => _sqlChangeTrackingBuilderProcedure.GetDropStoredProcedureCommandAsync(storedProcedureType, filter, connection, transaction);

        public override Task<DbCommand> GetCreateTrackingTableCommandAsync(DbConnection connection, DbTransaction transaction)
            => _sqlChangeTrackingBuilderTrackingTable.GetCreateTrackingTableCommandAsync(connection, transaction);
        public override Task<DbCommand> GetDropTrackingTableCommandAsync(DbConnection connection, DbTransaction transaction)
            => _sqlChangeTrackingBuilderTrackingTable.GetDropTrackingTableCommandAsync(connection, transaction);
        public override Task<DbCommand> GetRenameTrackingTableCommandAsync(ParserName oldTableName, DbConnection connection, DbTransaction transaction)
            => _sqlChangeTrackingBuilderTrackingTable.GetRenameTrackingTableCommandAsync(oldTableName, connection, transaction);
        public override Task<DbCommand> GetExistsTrackingTableCommandAsync(DbConnection connection, DbTransaction transaction)
            => _sqlChangeTrackingBuilderTrackingTable.GetExistsTrackingTableCommandAsync(connection, transaction);

        public override Task<DbCommand> GetExistsTriggerCommandAsync(DbTriggerType triggerType, DbConnection connection, DbTransaction transaction)
            => _sqlChangeTrackingBuilderTrigger.GetExistsTriggerCommandAsync(triggerType, connection, transaction);
        public override Task<DbCommand> GetCreateTriggerCommandAsync(DbTriggerType triggerType, DbConnection connection, DbTransaction transaction)
            => _sqlChangeTrackingBuilderTrigger.GetCreateTriggerCommandAsync(triggerType, connection, transaction);
        public override Task<DbCommand> GetDropTriggerCommandAsync(DbTriggerType triggerType, DbConnection connection, DbTransaction transaction)
            => _sqlChangeTrackingBuilderTrigger.GetDropTriggerCommandAsync(triggerType, connection, transaction);

    }
}
