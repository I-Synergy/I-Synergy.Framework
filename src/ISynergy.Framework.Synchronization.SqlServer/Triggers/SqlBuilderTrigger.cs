using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Model.Parsers;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Builders;
using ISynergy.Framework.Synchronization.SqlServer.Utilities;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Triggers
{
    /// <summary>
    /// SQL builder trigger.
    /// </summary>
    public class SqlBuilderTrigger
    {
        private ParserName _tableName;
        private ParserName _trackingName;
        private readonly SyncTable _tableDescription;
        private readonly SyncSetup _setup;
        private readonly SqlObjectNames _sqlObjectNames;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="tableDescription"></param>
        /// <param name="tableName"></param>
        /// <param name="trackingName"></param>
        /// <param name="setup"></param>
        public SqlBuilderTrigger(SyncTable tableDescription, ParserName tableName, ParserName trackingName, SyncSetup setup)
        {
            _tableDescription = tableDescription;
            _setup = setup;
            _tableName = tableName;
            _trackingName = trackingName;
            _sqlObjectNames = new SqlObjectNames(tableDescription, tableName, trackingName, setup);
        }

        private string CreateTrigger(DbCommandType commandType)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("SET NOCOUNT ON;");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("UPDATE [side] ");

            if(commandType == DbCommandType.DeleteTrigger){
                stringBuilder.AppendLine("SET [sync_row_is_tombstone] = 1");
            }
            else if(commandType == DbCommandType.InsertTrigger){
                stringBuilder.AppendLine("SET [sync_row_is_tombstone] = 0");
            }

            stringBuilder.AppendLine("\t,[update_scope_id] = NULL -- scope id is always NULL when update is made locally");
            stringBuilder.AppendLine("\t,[last_change_datetime] = GetUtcDate() ");
            stringBuilder.AppendLine($" FROM {_trackingName.Schema().Quoted().ToString()} [side] ");
            
            if(commandType == DbCommandType.DeleteTrigger){
                stringBuilder.Append($" JOIN DELETED AS [a] ON ");
            }
            else if(commandType == DbCommandType.InsertTrigger || commandType == DbCommandType.UpdateTrigger){
                stringBuilder.Append($" JOIN INSERTED AS [a] ON ");
            }

            stringBuilder.AppendLine(SqlManagementUtils.JoinTwoTablesOnClause(_tableDescription.PrimaryKeys, "[side]", "[a]"));
            stringBuilder.AppendLine();

            if(commandType == DbCommandType.UpdateTrigger){
                if (_tableDescription.GetMutableColumns().Count() > 0)
                {
                    stringBuilder.Append($" JOIN DELETED AS [b] ON ");
                    stringBuilder.AppendLine(SqlManagementUtils.JoinTwoTablesOnClause(_tableDescription.PrimaryKeys, "[b]", "[a]"));

                    stringBuilder.AppendLine(" WHERE (");
                    var or = "";
                    foreach (var column in _tableDescription.GetMutableColumns())
                    {
                        var quotedColumn = ParserName.Parse(column).Quoted().ToString();

                        stringBuilder.Append("\t");
                        stringBuilder.Append(or);
                        stringBuilder.Append("ISNULL(");
                        stringBuilder.Append("NULLIF(");
                        stringBuilder.Append("[b].");
                        stringBuilder.Append(quotedColumn);
                        stringBuilder.Append(", ");
                        stringBuilder.Append("[a].");
                        stringBuilder.Append(quotedColumn);
                        stringBuilder.Append(")");
                        stringBuilder.Append(", ");
                        stringBuilder.Append("NULLIF(");
                        stringBuilder.Append("[a].");
                        stringBuilder.Append(quotedColumn);
                        stringBuilder.Append(", ");
                        stringBuilder.Append("[b].");
                        stringBuilder.Append(quotedColumn);
                        stringBuilder.Append(")");
                        stringBuilder.AppendLine(") IS NOT NULL");

                        or = " OR ";
                    }
                    stringBuilder.AppendLine(") ");
                }
            }

            stringBuilder.AppendLine($"INSERT INTO {_trackingName.Schema().Quoted().ToString()} (");

            var stringBuilderArguments = new StringBuilder();
            var stringBuilderArguments2 = new StringBuilder();
            var stringPkAreNull = new StringBuilder();

            var argComma = " ";
            var argAnd = string.Empty;
            var primaryKeys = _tableDescription.GetPrimaryKeysColumns();

            foreach (var mutableColumn in primaryKeys.Where(c => !c.IsReadOnly))
            {
                var columnName = ParserName.Parse(mutableColumn).Quoted().ToString();
                stringBuilderArguments.AppendLine($"\t{argComma}[a].{columnName}");
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
            
            if(commandType == DbCommandType.DeleteTrigger){
                stringBuilder.AppendLine("\t,1");
            }
            else if(commandType == DbCommandType.InsertTrigger || commandType == DbCommandType.UpdateTrigger){
                stringBuilder.AppendLine("\t,0");
            }
            
            stringBuilder.AppendLine("\t,GetUtcDate()");

            if(commandType == DbCommandType.DeleteTrigger){
                stringBuilder.AppendLine("FROM DELETED [a]");
            }
            else if(commandType == DbCommandType.InsertTrigger || commandType == DbCommandType.UpdateTrigger){
                stringBuilder.AppendLine("FROM INSERTED [a]");
            }
            
            if(commandType == DbCommandType.UpdateTrigger){
                stringBuilder.Append($"JOIN DELETED AS [b] ON ");
                stringBuilder.AppendLine(SqlManagementUtils.JoinTwoTablesOnClause(_tableDescription.PrimaryKeys, "[b]", "[a]"));
            }

            stringBuilder.Append($"LEFT JOIN {_trackingName.Schema().Quoted().ToString()} [side] ON ");
            stringBuilder.AppendLine(SqlManagementUtils.JoinTwoTablesOnClause(_tableDescription.PrimaryKeys, "[a]", "[side]"));
            stringBuilder.Append("WHERE ");
            stringBuilder.AppendLine(stringPkAreNull.ToString());

            if(commandType == DbCommandType.UpdateTrigger){
                if (_tableDescription.GetMutableColumns().Count() > 0)
                {
                    stringBuilder.AppendLine("AND (");
                    var or = "";
                    foreach (var column in _tableDescription.GetMutableColumns())
                    {
                        var quotedColumn = ParserName.Parse(column).Quoted().ToString();

                        stringBuilder.Append("\t");
                        stringBuilder.Append(or);
                        stringBuilder.Append("ISNULL(");
                        stringBuilder.Append("NULLIF(");
                        stringBuilder.Append("[b].");
                        stringBuilder.Append(quotedColumn);
                        stringBuilder.Append(", ");
                        stringBuilder.Append("[a].");
                        stringBuilder.Append(quotedColumn);
                        stringBuilder.Append(")");
                        stringBuilder.Append(", ");
                        stringBuilder.Append("NULLIF(");
                        stringBuilder.Append("[a].");
                        stringBuilder.Append(quotedColumn);
                        stringBuilder.Append(", ");
                        stringBuilder.Append("[b].");
                        stringBuilder.Append(quotedColumn);
                        stringBuilder.Append(")");
                        stringBuilder.AppendLine(") IS NOT NULL");

                        or = " OR ";
                    }
                    stringBuilder.AppendLine(") ");
                }
            }

            return stringBuilder.ToString();
        }

        private string CreateDeleteTriggerAsync() =>
            CreateTrigger(DbCommandType.DeleteTrigger);

        private string CreateInsertTriggerAsync() =>
            CreateTrigger(DbCommandType.InsertTrigger);

        private string CreateUpdateTriggerAsync() =>
            CreateTrigger(DbCommandType.UpdateTrigger);

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
            var triggerFor = triggerType == DbTriggerType.Delete ? "DELETE"
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