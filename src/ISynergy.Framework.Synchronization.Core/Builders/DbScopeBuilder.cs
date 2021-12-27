using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;

namespace ISynergy.Framework.Synchronization.Core.Builders
{
    public abstract class DbScopeBuilder
    {
        public ParserName ScopeInfoTableName { get; protected set; }

        public DbScopeBuilder(string scopeInfoTableName) => ScopeInfoTableName = ParserName.Parse(scopeInfoTableName);

        public abstract DbCommand GetExistsScopeInfoTableCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction);
        public abstract DbCommand GetCreateScopeInfoTableCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction);
        public abstract DbCommand GetAllScopesCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction);
        public abstract DbCommand GetInsertScopeInfoCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction);
        public abstract DbCommand GetUpdateScopeInfoCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction);
        public abstract DbCommand GetLocalTimestampCommand(DbConnection connection, DbTransaction transaction);
        public abstract DbCommand GetDropScopeInfoTableCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction);
        public abstract DbCommand GetExistsScopeInfoCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction);



        // Internal commands cache
        private ConcurrentDictionary<string, Lazy<SyncCommand>> commands = new ConcurrentDictionary<string, Lazy<SyncCommand>>();

        /// <summary>
        /// Remove a Command from internal shared dictionary
        /// </summary>
        internal void RemoveCommands() => commands.Clear();

        /// <summary>
        /// Get the command from provider, check connection is opened, affect connection and transaction
        /// Prepare the command parameters and add scope parameters
        /// </summary>
        internal DbCommand GetCommandAsync(DbScopeCommandType commandType, DbScopeType scopeType, DbConnection connection, DbTransaction transaction, SyncFilter filter = null)
        {
            if (connection is null)
                throw new MissingConnectionException();

            // Create the key
            var commandKey = $"{connection.DataSource}-{connection.Database}-{ScopeInfoTableName.ToString()}-{commandType}-{scopeType}";

            var command = commandType switch
            {
                DbScopeCommandType.ExistsScopeTable => GetExistsScopeInfoTableCommand(scopeType, connection, transaction),
                DbScopeCommandType.CreateScopeTable => GetCreateScopeInfoTableCommand(scopeType, connection, transaction),
                DbScopeCommandType.GetScopes => GetAllScopesCommand(scopeType, connection, transaction),
                DbScopeCommandType.InsertScope => GetInsertScopeInfoCommand(scopeType, connection, transaction),
                DbScopeCommandType.UpdateScope => GetUpdateScopeInfoCommand(scopeType, connection, transaction),
                DbScopeCommandType.ExistScope => GetExistsScopeInfoCommand(scopeType, connection, transaction),
                DbScopeCommandType.GetLocalTimestamp => GetLocalTimestampCommand(connection, transaction),
                DbScopeCommandType.DropScopeTable => GetDropScopeInfoTableCommand(scopeType, connection, transaction),
                _ => throw new Exception($"This DbScopeCommandType {commandType} not exists")
            };

            if (command is not null)
            {
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
        /// Sets the generic scope parameters.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public DbCommand SetScopeParameters(DbCommand command)
        {
            var p = command.CreateParameter();
            p.ParameterName = "@sync_scope_name";
            p.DbType = DbType.String;
            p.Size = 100;
            command.Parameters.Add(p);

            p = command.CreateParameter();
            p.ParameterName = "@sync_scope_schema";
            p.DbType = DbType.String;
            p.Size = int.MaxValue;
            command.Parameters.Add(p);

            p = command.CreateParameter();
            p.ParameterName = "@sync_scope_setup";
            p.DbType = DbType.String;
            p.Size = int.MaxValue;
            command.Parameters.Add(p);

            p = command.CreateParameter();
            p.ParameterName = "@sync_scope_version";
            p.DbType = DbType.String;
            p.Size = 10;
            command.Parameters.Add(p);

            return command;
        }
    }
}
