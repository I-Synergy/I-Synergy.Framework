using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Sqlite.Models;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;
using System.Text;

namespace ISynergy.Framework.Synchronization.Sqlite.Builders
{
    public class SqliteScopeBuilder : DbScopeBuilder
    {
        public SqliteScopeBuilder(string scopeInfoTableName) : base(scopeInfoTableName)
        {
        }

        public override DbCommand GetAllScopesCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction)
        {
            if (scopeType != DbScopeType.Client)
                return null;

            var commandText =
                                $@"SELECT sync_scope_id
                           , sync_scope_name
                           , sync_scope_schema
                           , sync_scope_setup
                           , sync_scope_version
                           , scope_last_sync
                           , scope_last_server_sync_timestamp
                           , scope_last_sync_timestamp
                           , scope_last_sync_duration
                    FROM  {ScopeInfoTableName.Unquoted().ToString()}
                    WHERE sync_scope_name = @sync_scope_name";

            var command = connection.CreateCommand();

            command.CommandText = commandText;

            command.Connection = connection;
            command.Transaction = transaction;

            var p = command.CreateParameter();
            p.ParameterName = "@sync_scope_name";
            p.DbType = DbType.String;
            p.Size = 100;
            command.Parameters.Add(p);

            return command;
        }



        public override DbCommand GetCreateScopeInfoTableCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction)
        {
            if (scopeType != DbScopeType.Client)
                return null;

            var commandText =
                    $@"CREATE TABLE {ScopeInfoTableName.Quoted().ToString()}(
                        sync_scope_id blob NOT NULL PRIMARY KEY,
	                    sync_scope_name text NOT NULL,
	                    sync_scope_schema text NULL,
	                    sync_scope_setup text NULL,
	                    sync_scope_version text NULL,
                        scope_last_server_sync_timestamp integer NULL,
                        scope_last_sync_timestamp integer NULL,
                        scope_last_sync_duration integer NULL,
                        scope_last_sync datetime NULL
                        )";

            var command = connection.CreateCommand();

            command.Transaction = transaction;

            command.CommandText = commandText;

            return command;
        }

        public override DbCommand GetDropScopeInfoTableCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction)
        {
            if (scopeType != DbScopeType.Client)
                return null;

            var commandText = $"DROP Table {ScopeInfoTableName.Unquoted().ToString()}";

            var command = connection.CreateCommand();

            command.CommandText = commandText;

            command.Connection = connection;
            command.Transaction = transaction;

            return command;
        }

        public override DbCommand GetExistsScopeInfoCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction)
        {
            if (scopeType != DbScopeType.Client)
                return null;

            var commandText = $@"Select count(*) from {ScopeInfoTableName.Unquoted().ToString()} where sync_scope_id = @sync_scope_id";

            var scommand = new SqliteCommand(commandText, (SqliteConnection)connection, (SqliteTransaction)transaction);

            var p0 = scommand.CreateParameter();
            p0.ParameterName = "@sync_scope_id";
            p0.DbType = DbType.String;
            scommand.Parameters.Add(p0);

            return scommand;

        }

        public override DbCommand GetExistsScopeInfoTableCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction)
        {
            if (scopeType != DbScopeType.Client)
                return null;

            var commandText = $@"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{ScopeInfoTableName.Unquoted().ToString()}'";

            var command = connection.CreateCommand();

            command.CommandText = commandText;

            command.Connection = connection;
            command.Transaction = transaction;

            return command;

        }

        public override DbCommand GetInsertScopeInfoCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction)
        {
            if (scopeType != DbScopeType.Client)
                return null;

            return GetSaveScopeInfoCommand(false, connection, transaction);
        }

        public override DbCommand GetUpdateScopeInfoCommand(DbScopeType scopeType, DbConnection connection, DbTransaction transaction)
        {
            if (scopeType != DbScopeType.Client)
                return null;

            return GetSaveScopeInfoCommand(true, connection, transaction);
        }

        public override DbCommand GetLocalTimestampCommand(DbConnection connection, DbTransaction transaction)
        {

            var commandText = $"Select {SqliteObjectNames.TimestampValue}";

            var command = connection.CreateCommand();

            command.CommandText = commandText;

            command.Connection = connection;
            command.Transaction = transaction;

            return command;
        }


        public DbCommand GetSaveScopeInfoCommand(bool exist, DbConnection connection, DbTransaction transaction)
        {
            var stmtText = new StringBuilder();

            stmtText.AppendLine(exist
                    ? $"Update {ScopeInfoTableName.Unquoted().ToString()} set sync_scope_name=@sync_scope_name, sync_scope_schema=@sync_scope_schema, sync_scope_setup=@sync_scope_setup, sync_scope_version=@sync_scope_version, scope_last_sync=@scope_last_sync, scope_last_server_sync_timestamp=@scope_last_server_sync_timestamp,  scope_last_sync_timestamp=@scope_last_sync_timestamp, scope_last_sync_duration=@scope_last_sync_duration where sync_scope_id=@sync_scope_id;"
                    : $"Insert into {ScopeInfoTableName.Unquoted().ToString()} (sync_scope_name, sync_scope_schema, sync_scope_setup, sync_scope_version, scope_last_sync, scope_last_sync_duration, scope_last_server_sync_timestamp, scope_last_sync_timestamp, sync_scope_id) values (@sync_scope_name, @sync_scope_schema, @sync_scope_setup, @sync_scope_version, @scope_last_sync, @scope_last_sync_duration, @scope_last_server_sync_timestamp, @scope_last_sync_timestamp, @sync_scope_id);");


            stmtText.AppendLine(@$"SELECT sync_scope_id
                           , sync_scope_name
                           , sync_scope_schema
                           , sync_scope_setup
                           , sync_scope_version
                           , scope_last_sync
                           , scope_last_server_sync_timestamp
                           , scope_last_sync_timestamp
                           , scope_last_sync_duration
                    FROM  {ScopeInfoTableName.Unquoted().ToString()}
                    WHERE sync_scope_name = @sync_scope_name");

            var command = new SqliteCommand(stmtText.ToString(), (SqliteConnection)connection, (SqliteTransaction)transaction);

            var p = command.CreateParameter();
            p.ParameterName = "@scope_last_sync";
            p.DbType = DbType.DateTime;
            command.Parameters.Add(p);

            p = command.CreateParameter();
            p.ParameterName = "@scope_last_server_sync_timestamp";
            p.DbType = DbType.Int64;
            command.Parameters.Add(p);

            p = command.CreateParameter();
            p.ParameterName = "@scope_last_sync_timestamp";
            p.DbType = DbType.Int64;
            command.Parameters.Add(p);

            p = command.CreateParameter();
            p.ParameterName = "@scope_last_sync_duration";
            p.DbType = DbType.Int64;
            command.Parameters.Add(p);

            p = command.CreateParameter();
            p.ParameterName = "@sync_scope_id";
            p.DbType = DbType.String;
            p.Size = -1;
            command.Parameters.Add(p);

            return SetScopeParameters(command);
        }
    }
}
