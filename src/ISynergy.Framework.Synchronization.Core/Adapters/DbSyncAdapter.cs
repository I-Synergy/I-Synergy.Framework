using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Adapters
{
    /// <summary>
    /// The SyncAdapter is the datasource manager for ONE table
    /// Should be implemented by every database provider and provide every SQL action
    /// </summary>
    public abstract class DbSyncAdapter
    {
        internal const int BATCH_SIZE = 10000;


        // Internal commands cache
        private ConcurrentDictionary<string, Lazy<SyncCommand>> commands = new ConcurrentDictionary<string, Lazy<SyncCommand>>();

        /// <summary>
        /// Gets the table description
        /// </summary>
        public SyncTable TableDescription { get; private set; }

        /// <summary>
        /// Gets the setup used 
        /// </summary>
        public SyncSetup Setup { get; }

        /// <summary>
        /// Get or Set the current step (could be only Added, Modified, Deleted)
        /// </summary>
        internal DataRowState ApplyType { get; set; }

        /// <summary>
        /// Get if the error is a primarykey exception
        /// </summary>
        public abstract bool IsPrimaryKeyViolation(Exception exception);

        /// <summary>
        /// Gets if the error is a unique key constraint exception
        /// </summary>
        public abstract bool IsUniqueKeyViolation(Exception exception);

        /// <summary>
        /// Gets a command from the current adapter
        /// </summary>
        public abstract DbCommand GetCommand(DbCommandType commandType, SyncFilter filter = null);

        /// <summary>
        /// Add parameters to a command
        /// </summary>
        public abstract Task AddCommandParametersAsync(DbCommandType commandType, DbCommand command, DbConnection connection, DbTransaction transaction, SyncFilter filter = null);

        /// <summary>
        /// Execute a batch command
        /// </summary>
        public abstract Task ExecuteBatchCommandAsync(DbCommand cmd, Guid senderScopeId, IEnumerable<SyncRow> arrayItems, SyncTable schemaChangesTable,
                                                      SyncTable failedRows, long? lastTimestamp, DbConnection connection, DbTransaction transaction);

        /// <summary>
        /// Create a Sync Adapter
        /// </summary>
        public DbSyncAdapter(SyncTable tableDescription, SyncSetup setup)
        {
            TableDescription = tableDescription;
            Setup = setup;
        }

        /// <summary>
        /// Set command parameters value mapped to Row
        /// </summary>
        internal void SetColumnParametersValues(DbCommand command, SyncRow row)
        {
            if (row.Table is null)
                throw new ArgumentException("Schema table columns does not correspond to row values");

            var schemaTable = row.Table;

            foreach (DbParameter parameter in command.Parameters)
                if (!string.IsNullOrEmpty(parameter.SourceColumn))
                {
                    // foreach parameter, check if we have a column 
                    var column = schemaTable.Columns[parameter.SourceColumn];

                    if (column != null)
                    {
                        var value = row[column] ?? DBNull.Value;
                        SetParameterValue(command, parameter.ParameterName, value);
                    }
                }

            // return value
            var syncRowCountParam = GetParameter(command, "sync_row_count");

            if (syncRowCountParam != null)
            {
                syncRowCountParam.Direction = ParameterDirection.Output;
                syncRowCountParam.Value = DBNull.Value;
            }
        }

        /// <summary>
        /// Remove a Command from internal shared dictionary
        /// </summary>
        internal void RemoveCommands() => commands.Clear();

        /// <summary>
        /// Get the command from provider, check connection is opened, affect connection and transaction
        /// Prepare the command parameters and add scope parameters
        /// </summary>
        public async Task<DbCommand> GetCommandAsync(DbCommandType commandType, DbConnection connection, DbTransaction transaction, SyncFilter filter = null)
        {
            if (connection is null)
                throw new MissingConnectionException();

            // Create the key
            var commandKey = $"{connection.DataSource}-{connection.Database}-{TableDescription.GetFullName()}-{commandType}";

            if(GetCommand(commandType, filter) is DbCommand command)
            {
                // Add Parameters
                await AddCommandParametersAsync(commandType, command, connection, transaction, filter).ConfigureAwait(false);

                if (connection.State != ConnectionState.Open)
                    throw new ConnectionClosedException(connection);

                command.Connection = connection;
                command.Transaction = transaction;

                // Get a lazy command instance
                var lazyCommand = commands.GetOrAdd(commandKey, k => new Lazy<SyncCommand>(() =>
                {
                    var syncCommand = new SyncCommand(commandKey);
                    return syncCommand;
                }));

                // lazyCommand.Metadata is a boolean indicating if the command is already prepared on the server
                if (lazyCommand.Value.IsPrepared == true)
                    return command;

                // Testing The Prepare() performance increase
                command.Prepare();

                // Adding this command as prepared
                lazyCommand.Value.IsPrepared = true;

                commands.AddOrUpdate(commandKey, lazyCommand, (key, lc) => new Lazy<SyncCommand>(() => lc.Value));

                return command;
            }

            throw new MissingCommandException(commandType.ToString());
        }

        /// <summary>
        /// Add common parameters which could be part of the command
        /// if not found, no set done
        /// </summary>
        internal void AddScopeParametersValues(DbCommand command, Guid? id, long? lastTimestamp, bool isDeleted, bool forceWrite)
        {
            // ISynergy.Framework.Synchronization.Core parameters
            SetParameterValue(command, "sync_force_write", forceWrite ? 1 : 0);
            SetParameterValue(command, "sync_min_timestamp", lastTimestamp.HasValue ? lastTimestamp.Value : DBNull.Value);
            SetParameterValue(command, "sync_scope_id", id.HasValue ? id.Value : DBNull.Value);
            SetParameterValue(command, "sync_row_is_tombstone", isDeleted);
        }


        // TODO : Migrate to BaseOrchestrator
        /// <summary>
        /// Create a change table with scope columns and tombstone column
        /// </summary>
        public static SyncTable CreateChangesTable(SyncTable syncTable, SyncSet owner)
        {
            if (syncTable.Schema is null)
                throw new ArgumentException("Schema can't be null when creating a changes table");

            // Create an empty sync table without columns
            var changesTable = new SyncTable(syncTable.TableName, syncTable.SchemaName)
            {
                OriginalProvider = syncTable.OriginalProvider,
                SyncDirection = syncTable.SyncDirection
            };

            // Adding primary keys
            foreach (var pkey in syncTable.PrimaryKeys)
                changesTable.PrimaryKeys.Add(pkey);

            // get ordered columns that are mutables and pkeys
            var orderedNames = syncTable.GetMutableColumnsWithPrimaryKeys();

            foreach (var c in orderedNames)
                changesTable.Columns.Add(c.Clone());

            owner.Tables.Add(changesTable);

            return changesTable;
        }


        /// <summary>
        /// Get a parameter even if it's a @param or :param or param
        /// </summary>
        public static DbParameter GetParameter(DbCommand command, string parameterName)
        {
            if (command is null)
                return null;

            if (command.Parameters.Contains($"@{parameterName}"))
                return command.Parameters[$"@{parameterName}"];

            if (command.Parameters.Contains($":{parameterName}"))
                return command.Parameters[$":{parameterName}"];

            if (command.Parameters.Contains($"in_{parameterName}"))
                return command.Parameters[$"in_{parameterName}"];

            if (!command.Parameters.Contains(parameterName))
                return null;

            return command.Parameters[parameterName];
        }

        /// <summary>
        /// Set a parameter value
        /// </summary>
        public static void SetParameterValue(DbCommand command, string parameterName, object value)
        {
            var parameter = GetParameter(command, parameterName);
            if (parameter is null)
                return;

            if (value is null || value == DBNull.Value)
                parameter.Value = DBNull.Value;
            else
                parameter.Value = SyncTypeConverter.TryConvertFromDbType(value, parameter.DbType);


        }

        /// <summary>
        /// Get Sync integer parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static int GetSyncIntOutParameter(string parameter, DbCommand command)
        {
            var dbParameter = GetParameter(command, parameter);
            if (dbParameter is null || dbParameter.Value is null || string.IsNullOrEmpty(dbParameter.Value.ToString()))
                return 0;

            return int.Parse(dbParameter.Value.ToString(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parse a time stamp value
        /// </summary>
        public static long ParseTimestamp(object obj)
        {
            if (obj == DBNull.Value)
                return 0;

            if (obj is long || obj is int || obj is ulong || obj is uint || obj is decimal)
                return Convert.ToInt64(obj, NumberFormatInfo.InvariantInfo);
            long timestamp;
            if (obj is string str)
            {
                long.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out timestamp);
                return timestamp;
            }

            if (!(obj is byte[] numArray))
                return 0;

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < numArray.Length; i++)
            {
                var str1 = numArray[i].ToString("X", NumberFormatInfo.InvariantInfo);
                stringBuilder.Append(str1.Length == 1 ? string.Concat("0", str1) : str1);
            }

            long.TryParse(stringBuilder.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out timestamp);
            return timestamp;
        }


    }
}
