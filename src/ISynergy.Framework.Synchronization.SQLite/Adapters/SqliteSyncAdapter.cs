using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Model.Parsers;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.Sqlite.Builders;
using ISynergy.Framework.Synchronization.Sqlite.Metadata;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Sqlite.Adapters
{
    /// <summary>
    /// Sqlite sync adapter.
    /// </summary>
    public class SqliteSyncAdapter : DbSyncAdapter
    {
        private readonly SqliteObjectNames _sqliteObjectNames;
        private readonly SqliteDbMetadata _sqliteDbMetadata;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="tableDescription"></param>
        /// <param name="tableName"></param>
        /// <param name="trackingName"></param>
        /// <param name="setup"></param>
        public SqliteSyncAdapter(SyncTable tableDescription, ParserName tableName, ParserName trackingName, SyncSetup setup) 
            : base(tableDescription, setup)
        {
            _sqliteObjectNames = new SqliteObjectNames(tableDescription, tableName, trackingName, setup);
            _sqliteDbMetadata = new SqliteDbMetadata();
        }

        /// <summary>
        /// Gets true if primary key is violated.
        /// </summary>
        /// <param name="Error"></param>
        /// <returns></returns>
        public override bool IsPrimaryKeyViolation(Exception Error) => false;

        /// <summary>
        /// Gets true if unique key is violated.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override bool IsUniqueKeyViolation(Exception exception) => false;

        /// <summary>
        /// Gets command.
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override DbCommand GetCommand(DbCommandType commandType, SyncFilter filter = null)
        {
            var command = new SqliteCommand();
            var text = _sqliteObjectNames.GetCommandName(commandType, filter);

            // on Sqlite, everything is text :)
            command.CommandType = CommandType.Text;
            command.CommandText = text;

            return command;
        }

        /// <summary>
        /// Adds parameters to command.
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="command"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override Task AddCommandParametersAsync(DbCommandType commandType, DbCommand command, DbConnection connection, DbTransaction transaction = null, SyncFilter filter = null)
        {

            if (command == null)
                return Task.CompletedTask;

            if (command.Parameters != null && command.Parameters.Count > 0)
                return Task.CompletedTask;

            switch (commandType)
            {
                case DbCommandType.SelectChanges:
                case DbCommandType.SelectChangesWithFilters:
                    this.SetSelecteChangesParameters(command);
                    break;
                case DbCommandType.SelectRow:
                    this.SetSelectRowParameters(command);
                    break;
                case DbCommandType.DeleteMetadata:
                    this.SetDeleteMetadataParameters(command);
                    break;
                case DbCommandType.DeleteRow:
                    this.SetDeleteRowParameters(command);
                    break;
                case DbCommandType.UpdateRow:
                    this.SetUpdateRowParameters(command);
                    break;
                case DbCommandType.InitializeRow:
                    this.SetInitializeRowParameters(command);
                    break;
                case DbCommandType.Reset:
                    this.SetResetParameters(command);
                    break;
                case DbCommandType.UpdateMetadata:
                    this.SetUpdateMetadataParameters(command);
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        private void SetResetParameters(DbCommand command)
        {
            // nothing to set here
        }

        private DbType GetValidDbType(DbType dbType)
        {
            if (dbType == DbType.Time)
                return DbType.String;

            if (dbType == DbType.Object)
                return DbType.String;

            return dbType;
        }

        private void SetRowParameters(DbCommand command)
        {
            var p = command.CreateParameter();
            p.ParameterName = "@sync_force_write";
            p.DbType = DbType.Int64;
            command.Parameters.Add(p);

            p = command.CreateParameter();
            p.ParameterName = "@sync_min_timestamp";
            p.DbType = DbType.Int64;
            command.Parameters.Add(p);

            p = command.CreateParameter();
            p.ParameterName = "@sync_scope_id";
            p.DbType = DbType.Guid;
            command.Parameters.Add(p);
        }

        private void SetUpdateRowParameters(DbCommand command) =>
            SetInitializeRowParameters(command);

        private void SetInitializeRowParameters(DbCommand command)
        {
            foreach (var column in this.TableDescription.Columns.Where(c => !c.IsReadOnly))
            {
                var unquotedColumn = ParserName.Parse(column).Normalized().Unquoted().ToString();
                var p = command.CreateParameter();
                p.ParameterName = $"@{unquotedColumn}";
                p.DbType = GetValidDbType(column.GetDbType());
                p.SourceColumn = column.ColumnName;
                command.Parameters.Add(p);
            }

            SetRowParameters(command);
        }

        private void SetDeleteRowParameters(DbCommand command)
        {
            SetSelectRowParameters(command);    
            SetRowParameters(command);
        }

        private void SetSelectRowParameters(DbCommand command)
        {
            foreach (var column in this.TableDescription.GetPrimaryKeysColumns().Where(c => !c.IsReadOnly))
            {
                var unquotedColumn = ParserName.Parse(column).Normalized().Unquoted().ToString();
                var p = command.CreateParameter();
                p.ParameterName = $"@{unquotedColumn}";
                p.DbType = GetValidDbType(column.GetDbType());
                p.SourceColumn = column.ColumnName;
                command.Parameters.Add(p);
            }

            var param = command.CreateParameter();
            param.ParameterName = "@sync_scope_id";
            param.DbType = DbType.Guid;
            command.Parameters.Add(param);
        }

        private void SetUpdateMetadataParameters(DbCommand command)
        {
            SetSelectRowParameters(command);

            var p = command.CreateParameter();
            p.ParameterName = "@sync_row_is_tombstone";
            p.DbType = DbType.Boolean;
            command.Parameters.Add(p);

        }
        private void SetDeleteMetadataParameters(DbCommand command)
        {
            var p = command.CreateParameter();
            p.ParameterName = "@sync_row_timestamp";
            p.DbType = DbType.Int64;
            command.Parameters.Add(p);
        }

        private void SetSelecteChangesParameters(DbCommand command)
        {
            var p = command.CreateParameter();
            p.ParameterName = "@sync_min_timestamp";
            p.DbType = DbType.Int64;
            command.Parameters.Add(p);

            p = command.CreateParameter();
            p.ParameterName = "@sync_scope_id";
            p.DbType = DbType.Guid;
            command.Parameters.Add(p);
        }

        /// <summary>
        /// Execute batch command.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="senderScopeId"></param>
        /// <param name="applyRows"></param>
        /// <param name="schemaChangesTable"></param>
        /// <param name="failedRows"></param>
        /// <param name="lastTimestamp"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Task ExecuteBatchCommandAsync(DbCommand cmd, Guid senderScopeId, IEnumerable<SyncRow> applyRows, SyncTable schemaChangesTable, SyncTable failedRows, long? lastTimestamp, DbConnection connection, DbTransaction transaction = null)
            => throw new NotImplementedException();
    }
}
