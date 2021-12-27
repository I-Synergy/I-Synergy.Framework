


using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Models;
using ISynergy.Framework.Synchronization.SqlServer.Utilities;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Builders
{
    public class SqlBuilderTrigger
    {
        private ParserName _tableName;
        private ParserName _trackingName;
        private readonly SyncTable _tableDescription;
        private readonly SyncSetup _setup;
        private readonly SqlObjectNames _sqlObjectNames;

        public SqlBuilderTrigger(SyncTable tableDescription, ParserName tableName, ParserName trackingName, SyncSetup setup)
        {
            _tableDescription = tableDescription;
            _setup = setup;
            _tableName = tableName;
            _trackingName = trackingName;
            _sqlObjectNames = new SqlObjectNames(tableDescription, tableName, trackingName, setup);

        }

        private string CreateDeleteTriggerAsync()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("SET NOCOUNT ON;");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("UPDATE [side] ");
            stringBuilder.AppendLine("SET  [sync_row_is_tombstone] = 1");
            stringBuilder.AppendLine("\t,[update_scope_id] = NULL -- scope id is always NULL when update is made locally");
            stringBuilder.AppendLine("\t,[last_change_datetime] = GetUtcDate()");
            stringBuilder.AppendLine($"FROM {_trackingName.Schema().Quoted().ToString()} [side]");
            stringBuilder.Append($"JOIN DELETED AS [d] ON ");
            stringBuilder.AppendLine(SqlManagementUtils.JoinTwoTablesOnClause(_tableDescription.PrimaryKeys, "[side]", "[d]"));
            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"INSERT INTO {_trackingName.Schema().Quoted().ToString()} (");

            var stringBuilderArguments = new StringBuilder();
            var stringBuilderArguments2 = new StringBuilder();
            var stringPkAreNull = new StringBuilder();

            string argComma = " ";
            string argAnd = string.Empty;
            var primaryKeys = _tableDescription.GetPrimaryKeysColumns();

            foreach (var mutableColumn in primaryKeys.Where(c => !c.IsReadOnly))
            {
                var columnName = ParserName.Parse(mutableColumn).Quoted().ToString();
                stringBuilderArguments.AppendLine($"\t{argComma}[d].{columnName}");
                stringBuilderArguments2.AppendLine($"\t{argComma}{columnName}");
                stringPkAreNull.Append($"{argAnd}[side].{columnName} IS NULL");
                argComma = ",";
                argAnd = " AND ";
            }

            stringBuilder.Append(stringBuilderArguments2.ToString());
            stringBuilder.AppendLine("\t,[update_scope_id]");
            stringBuilder.AppendLine("\t,[sync_row_is_tombstone]");
            stringBuilder.AppendLine("\t,[last_change_datetime]");
            stringBuilder.AppendLine(") ");
            stringBuilder.AppendLine("SELECT");
            stringBuilder.Append(stringBuilderArguments.ToString());
            stringBuilder.AppendLine("\t,NULL");
            stringBuilder.AppendLine("\t,1");
            stringBuilder.AppendLine("\t,GetUtcDate()");
            stringBuilder.AppendLine("FROM DELETED [d]");
            stringBuilder.Append($"LEFT JOIN {_trackingName.Schema().Quoted().ToString()} [side] ON ");
            stringBuilder.AppendLine(SqlManagementUtils.JoinTwoTablesOnClause(_tableDescription.PrimaryKeys, "[d]", "[side]"));
            stringBuilder.Append("WHERE ");
            stringBuilder.AppendLine(stringPkAreNull.ToString());

            return stringBuilder.ToString();

        }

        private string CreateInsertTriggerAsync()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("SET NOCOUNT ON;");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("-- If row was deleted before, it already exists, so just make an update");
            stringBuilder.AppendLine("UPDATE [side] ");
            stringBuilder.AppendLine("SET  [sync_row_is_tombstone] = 0");
            stringBuilder.AppendLine("\t,[update_scope_id] = NULL -- scope id is always NULL when update is made locally");
            stringBuilder.AppendLine("\t,[last_change_datetime] = GetUtcDate()");
            stringBuilder.AppendLine($"FROM {_trackingName.Schema().Quoted().ToString()} [side]");
            stringBuilder.Append($"JOIN INSERTED AS [i] ON ");
            stringBuilder.AppendLine(SqlManagementUtils.JoinTwoTablesOnClause(_tableDescription.PrimaryKeys, "[side]", "[i]"));
            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"INSERT INTO {_trackingName.Schema().Quoted().ToString()} (");

            var stringBuilderArguments = new StringBuilder();
            var stringBuilderArguments2 = new StringBuilder();
            var stringPkAreNull = new StringBuilder();

            string argComma = " ";
            string argAnd = string.Empty;
            var primaryKeys = _tableDescription.GetPrimaryKeysColumns();

            foreach (var mutableColumn in primaryKeys.Where(c => !c.IsReadOnly))
            {
                var columnName = ParserName.Parse(mutableColumn).Quoted().ToString();
                stringBuilderArguments.AppendLine($"\t{argComma}[i].{columnName}");
                stringBuilderArguments2.AppendLine($"\t{argComma}{columnName}");
                stringPkAreNull.Append($"{argAnd}[side].{columnName} IS NULL");
                argComma = ",";
                argAnd = " AND ";
            }

            stringBuilder.Append(stringBuilderArguments2.ToString());
            stringBuilder.AppendLine("\t,[update_scope_id]");
            stringBuilder.AppendLine("\t,[sync_row_is_tombstone]");
            stringBuilder.AppendLine("\t,[last_change_datetime]");
            stringBuilder.AppendLine(") ");
            stringBuilder.AppendLine("SELECT");
            stringBuilder.Append(stringBuilderArguments.ToString());
            stringBuilder.AppendLine("\t,NULL");
            stringBuilder.AppendLine("\t,0");
            stringBuilder.AppendLine("\t,GetUtcDate()");
            stringBuilder.AppendLine("FROM INSERTED [i]");
            stringBuilder.Append($"LEFT JOIN {_trackingName.Schema().Quoted().ToString()} [side] ON ");
            stringBuilder.AppendLine(SqlManagementUtils.JoinTwoTablesOnClause(_tableDescription.PrimaryKeys, "[i]", "[side]"));
            stringBuilder.Append("WHERE ");
            stringBuilder.AppendLine(stringPkAreNull.ToString());


            return stringBuilder.ToString();

        }

        private string CreateUpdateTriggerAsync()
        {

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("SET NOCOUNT ON;");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("UPDATE [side] ");
            stringBuilder.AppendLine("SET \t[update_scope_id] = NULL -- since the update if from local, it's a NULL");
            stringBuilder.AppendLine("\t,[last_change_datetime] = GetUtcDate()");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"FROM {_trackingName.Schema().Quoted().ToString()} [side]");
            stringBuilder.Append($"JOIN INSERTED AS [i] ON ");
            stringBuilder.AppendLine(SqlManagementUtils.JoinTwoTablesOnClause(_tableDescription.PrimaryKeys, "[side]", "[i]"));

            if (_tableDescription.GetMutableColumns().Count() > 0)
            {
                stringBuilder.Append($"JOIN DELETED AS [d] ON ");
                stringBuilder.AppendLine(SqlManagementUtils.JoinTwoTablesOnClause(_tableDescription.PrimaryKeys, "[d]", "[i]"));

                stringBuilder.AppendLine("WHERE (");
                string or = "";
                foreach (var column in _tableDescription.GetMutableColumns())
                {
                    var quotedColumn = ParserName.Parse(column).Quoted().ToString();

                    stringBuilder.Append("\t");
                    stringBuilder.Append(or);
                    stringBuilder.Append("ISNULL(");
                    stringBuilder.Append("NULLIF(");
                    stringBuilder.Append("[d].");
                    stringBuilder.Append(quotedColumn);
                    stringBuilder.Append(", ");
                    stringBuilder.Append("[i].");
                    stringBuilder.Append(quotedColumn);
                    stringBuilder.Append(")");
                    stringBuilder.Append(", ");
                    stringBuilder.Append("NULLIF(");
                    stringBuilder.Append("[i].");
                    stringBuilder.Append(quotedColumn);
                    stringBuilder.Append(", ");
                    stringBuilder.Append("[d].");
                    stringBuilder.Append(quotedColumn);
                    stringBuilder.Append(")");
                    stringBuilder.AppendLine(") IS NOT NULL");

                    or = " OR ";
                }
                stringBuilder.AppendLine(") ");
            }

            stringBuilder.AppendLine($"INSERT INTO {_trackingName.Schema().Quoted().ToString()} (");

            var stringBuilderArguments = new StringBuilder();
            var stringBuilderArguments2 = new StringBuilder();
            var stringPkAreNull = new StringBuilder();

            string argComma = " ";
            string argAnd = string.Empty;
            var primaryKeys = _tableDescription.GetPrimaryKeysColumns();

            foreach (var mutableColumn in primaryKeys.Where(c => !c.IsReadOnly))
            {
                var columnName = ParserName.Parse(mutableColumn).Quoted().ToString();
                stringBuilderArguments.AppendLine($"\t{argComma}[i].{columnName}");
                stringBuilderArguments2.AppendLine($"\t{argComma}{columnName}");
                stringPkAreNull.Append($"{argAnd}[side].{columnName} IS NULL");
                argComma = ",";
                argAnd = " AND ";
            }

            stringBuilder.Append(stringBuilderArguments2.ToString());
            stringBuilder.AppendLine("\t,[update_scope_id]");
            stringBuilder.AppendLine("\t,[sync_row_is_tombstone]");
            stringBuilder.AppendLine("\t,[last_change_datetime]");
            stringBuilder.AppendLine(") ");
            stringBuilder.AppendLine("SELECT");
            stringBuilder.Append(stringBuilderArguments.ToString());
            stringBuilder.AppendLine("\t,NULL");
            stringBuilder.AppendLine("\t,0");
            stringBuilder.AppendLine("\t,GetUtcDate()");
            stringBuilder.AppendLine("FROM INSERTED [i]");
            stringBuilder.Append($"JOIN DELETED AS [d] ON ");
            stringBuilder.AppendLine(SqlManagementUtils.JoinTwoTablesOnClause(_tableDescription.PrimaryKeys, "[d]", "[i]"));
            stringBuilder.Append($"LEFT JOIN {_trackingName.Schema().Quoted().ToString()} [side] ON ");
            stringBuilder.AppendLine(SqlManagementUtils.JoinTwoTablesOnClause(_tableDescription.PrimaryKeys, "[i]", "[side]"));
            stringBuilder.Append("WHERE ");
            stringBuilder.AppendLine(stringPkAreNull.ToString());

            if (_tableDescription.GetMutableColumns().Count() > 0)
            {
                stringBuilder.AppendLine("AND (");
                string or = "";
                foreach (var column in _tableDescription.GetMutableColumns())
                {
                    var quotedColumn = ParserName.Parse(column).Quoted().ToString();

                    stringBuilder.Append("\t");
                    stringBuilder.Append(or);
                    stringBuilder.Append("ISNULL(");
                    stringBuilder.Append("NULLIF(");
                    stringBuilder.Append("[d].");
                    stringBuilder.Append(quotedColumn);
                    stringBuilder.Append(", ");
                    stringBuilder.Append("[i].");
                    stringBuilder.Append(quotedColumn);
                    stringBuilder.Append(")");
                    stringBuilder.Append(", ");
                    stringBuilder.Append("NULLIF(");
                    stringBuilder.Append("[i].");
                    stringBuilder.Append(quotedColumn);
                    stringBuilder.Append(", ");
                    stringBuilder.Append("[d].");
                    stringBuilder.Append(quotedColumn);
                    stringBuilder.Append(")");
                    stringBuilder.AppendLine(") IS NOT NULL");

                    or = " OR ";
                }
                stringBuilder.AppendLine(") ");
            }

            return stringBuilder.ToString();

        }

        public virtual Task<DbCommand> GetExistsTriggerCommandAsync(DbTriggerType triggerType, DbConnection connection, DbTransaction transaction)
        {

            var commandTriggerName = _sqlObjectNames.GetTriggerCommandName(triggerType);
            var triggerName = ParserName.Parse(commandTriggerName).ToString();

            var commandText = $@"IF EXISTS (SELECT tr.name FROM sys.triggers tr  
                                            JOIN sys.tables t ON tr.parent_id = t.object_id 
                                            JOIN sys.schemas s ON t.schema_id = s.schema_id 
                                            WHERE tr.name = @triggerName and s.name = @schemaName) SELECT 1 ELSE SELECT 0";

            var command = connection.CreateCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = commandText;

            var p1 = command.CreateParameter();
            p1.ParameterName = "@triggerName";
            p1.Value = triggerName;
            command.Parameters.Add(p1);

            var p2 = command.CreateParameter();
            p2.ParameterName = "@schemaName";
            p2.Value = SqlManagementUtils.GetUnquotedSqlSchemaName(ParserName.Parse(commandTriggerName));
            command.Parameters.Add(p2);

            return Task.FromResult(command);

        }
        public virtual Task<DbCommand> GetDropTriggerCommandAsync(DbTriggerType triggerType, DbConnection connection, DbTransaction transaction)
        {

            var commandTriggerName = _sqlObjectNames.GetTriggerCommandName(triggerType);

            var commandText = $@"DROP TRIGGER {commandTriggerName}";

            var command = connection.CreateCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = commandText;

            return Task.FromResult(command);
        }
        public virtual Task<DbCommand> GetCreateTriggerCommandAsync(DbTriggerType triggerType, DbConnection connection, DbTransaction transaction)
        {

            var commandTriggerCommandString = triggerType switch
            {
                DbTriggerType.Delete => CreateDeleteTriggerAsync(),
                DbTriggerType.Insert => CreateInsertTriggerAsync(),
                DbTriggerType.Update => CreateUpdateTriggerAsync(),
                _ => throw new NotImplementedException()
            };
            string triggerFor = triggerType == DbTriggerType.Delete ? "DELETE"
                              : triggerType == DbTriggerType.Update ? "UPDATE"
                              : "INSERT";

            var commandTriggerName = _sqlObjectNames.GetTriggerCommandName(triggerType);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"CREATE TRIGGER {commandTriggerName} ON {_tableName.Schema().Quoted().ToString()} FOR {triggerFor} AS");
            stringBuilder.AppendLine(commandTriggerCommandString);

            var command = connection.CreateCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = stringBuilder.ToString();

            return Task.FromResult(command);
        }

    }
}