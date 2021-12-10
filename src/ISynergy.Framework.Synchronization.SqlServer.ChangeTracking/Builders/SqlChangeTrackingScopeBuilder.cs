using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.SqlServer.Builders;
using System;
using System.Data;
using System.Data.Common;

namespace ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Builders
{
    public class SqlChangeTrackingScopeBuilder : SqlScopeBuilder
    {
        public SqlChangeTrackingScopeBuilder(string scopeInfoTableName) : base(scopeInfoTableName)
        {
        }

        public override DbCommand GetLocalTimestampCommand(DbConnection connection, DbTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "SELECT CHANGE_TRACKING_CURRENT_VERSION()";
            return command;
        }

        public override DbCommand GetInsertScopeInfoCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction)
            => GetUpdateScopeInfoCommand(scopeType, connection, transaction);

        public override DbCommand GetUpdateScopeInfoCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction)
            => scopeType switch
            {
                DbScopeType.Client => GetSaveClientScopeInfoCommand(connection, transaction),
                DbScopeType.ServerHistory => GetSaveServerHistoryScopeInfoCommand(connection, transaction),
                DbScopeType.Server => GetSaveServerScopeInfoCommandForTrackingChange(connection, transaction),
                _ => throw new NotImplementedException($"Can't save this DbScopeType {scopeType}")
            };


        /// <summary>
        /// Get save server scopeinfo command for change tracking.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public DbCommand GetSaveServerScopeInfoCommandForTrackingChange(DbConnection connection, DbTransaction transaction) 
            => GetSaveServerScopeInfoCommand(connection, transaction);
    }
}
