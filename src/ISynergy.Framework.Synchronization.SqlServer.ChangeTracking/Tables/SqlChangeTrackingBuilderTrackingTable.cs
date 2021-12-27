using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Extensions;
using ISynergy.Framework.Synchronization.SqlServer.Metadata;
using ISynergy.Framework.Synchronization.SqlServer.Utilities;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Tables
{
    public class SqlChangeTrackingBuilderTrackingTable
    {
        private ParserName _tableName;
        private ParserName _trackingName;
        private readonly SyncTable _tableDescription;
        private readonly SyncSetup _setup;
        private readonly SqlDbMetadata _sqlDbMetadata;


        public SqlChangeTrackingBuilderTrackingTable(SyncTable tableDescription, ParserName tableName, ParserName trackingName, SyncSetup setup)
        {
            _tableDescription = tableDescription;
            _setup = setup;
            _tableName = tableName;
            _trackingName = trackingName;
            _sqlDbMetadata = new SqlDbMetadata();
        }

        public Task<DbCommand> GetExistsTrackingTableCommandAsync(DbConnection connection, DbTransaction transaction) =>
            connection.ToCommandAsync(transaction, _tableName);

        public Task<DbCommand> GetCreateTrackingTableCommandAsync(DbConnection connection, DbTransaction transaction)
        {
            var command = connection.CreateCommand();

            if (_setup.HasTableWithColumns(_tableDescription.TableName))
            {
                command.CommandText = $"ALTER TABLE {_tableName.Schema().Quoted().ToString()} ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = ON);";
            }
            else
            {
                command.CommandText = $"ALTER TABLE {_tableName.Schema().Quoted().ToString()} ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = OFF);";
            }

            command.Connection = connection;
            command.Transaction = transaction;

            return Task.FromResult(command);
        }

        public Task<DbCommand> GetDropTrackingTableCommandAsync(DbConnection connection, DbTransaction transaction)
        {
            var commandText = $"ALTER TABLE {_tableName.Schema().Quoted().ToString()} DISABLE CHANGE_TRACKING;";

            var command = connection.CreateCommand();
            command.Connection = connection;
            command.Transaction = transaction;
            command.CommandText = commandText;

            return Task.FromResult(command);
        }

        public Task<DbCommand> GetRenameTrackingTableCommandAsync(ParserName oldTableName, DbConnection connection, DbTransaction transaction)
            => Task.FromResult<DbCommand>(null);
    }
}
