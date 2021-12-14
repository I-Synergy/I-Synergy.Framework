﻿using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Model.Parsers;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.Sqlite.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISynergy.Framework.Synchronization.Sqlite.Builders
{
    public class SqliteObjectNames
    {
        public const string TimestampValue = "replace(strftime('%Y%m%d%H%M%f', 'now'), '.', '')";

        internal const string insertTriggerName = "[{0}_insert_trigger]";
        internal const string updateTriggerName = "[{0}_update_trigger]";
        internal const string deleteTriggerName = "[{0}_delete_trigger]";

        private Dictionary<DbCommandType, string> commandNames = new Dictionary<DbCommandType, string>();
        Dictionary<DbTriggerType, string> triggersNames = new Dictionary<DbTriggerType, string>();

        private ParserName tableName, trackingName;

        public SyncTable TableDescription { get; }
        public SyncSetup Setup { get; }

        public void AddCommandName(DbCommandType objectType, string name)
        {
            if (commandNames.ContainsKey(objectType))
                throw new Exception("Yous can't add an objectType multiple times");

            commandNames.Add(objectType, name);
        }
        public string GetCommandName(DbCommandType objectType, SyncFilter filter = null)
        {
            if (!commandNames.ContainsKey(objectType))
                throw new NotSupportedException($"Sqlite provider does not support the command type {objectType.ToString()}");

            var commandName = commandNames[objectType];

            // concat filter name
            //if (filter is not null)
            //    commandName = string.Format(commandName, filter.GetFilterName());

            return commandName;
        }


        public void AddTriggerName(DbTriggerType objectType, string name)
        {
            if (triggersNames.ContainsKey(objectType))
                throw new Exception("Yous can't add an objectType multiple times");

            triggersNames.Add(objectType, name);
        }
        public string GetTriggerCommandName(DbTriggerType objectType, SyncFilter filter = null)
        {
            if (!triggersNames.ContainsKey(objectType))
                throw new Exception("Yous should provide a value for all DbCommandName");

            var commandName = triggersNames[objectType];

            //// concat filter name
            //if (filter is not null)
            //    commandName = string.Format(commandName, filter.GetFilterName());

            return commandName;
        }



        public SqliteObjectNames(SyncTable tableDescription, ParserName tableName, ParserName trackingName, SyncSetup setup)
        {
            TableDescription = tableDescription;
            Setup = setup;
            tableName = tableName;
            trackingName = trackingName;

            SetDefaultNames();
        }

        /// <summary>
        /// Set the default stored procedures names
        /// </summary>
        private void SetDefaultNames()
        {
            var tpref = Setup.TriggersPrefix is not null ? Setup.TriggersPrefix : "";
            var tsuf = Setup.TriggersSuffix is not null ? Setup.TriggersSuffix : "";

            AddTriggerName(DbTriggerType.Insert, string.Format(insertTriggerName, $"{tpref}{tableName.Unquoted().Normalized().ToString()}{tsuf}"));
            AddTriggerName(DbTriggerType.Update, string.Format(updateTriggerName, $"{tpref}{tableName.Unquoted().Normalized().ToString()}{tsuf}"));
            AddTriggerName(DbTriggerType.Delete, string.Format(deleteTriggerName, $"{tpref}{tableName.Unquoted().Normalized().ToString()}{tsuf}"));

            // Select changes
            CreateSelectChangesCommandText();
            CreateSelectRowCommandText();
            CreateSelectInitializedCommandText();
            CreateDeleteCommandText();
            CreateDeleteMetadataCommandText();
            CreateUpdateCommandText();
            CreateInitializeCommandText();
            CreateResetCommandText();
            CreateUpdateUntrackedRowsCommandText();
            CreateUpdateMetadataCommandText();

            // Sqlite does not have any constraints, so just return a simple statement
            AddCommandName(DbCommandType.DisableConstraints, "Select 0"); // PRAGMA foreign_keys = OFF
            AddCommandName(DbCommandType.EnableConstraints, "Select 0");

        }

        private void CreateResetCommandText()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"DELETE FROM {tableName.Quoted().ToString()};");
            stringBuilder.AppendLine($"DELETE FROM {trackingName.Quoted().ToString()};");
            AddCommandName(DbCommandType.Reset, stringBuilder.ToString());
        }

        private void CreateUpdateMetadataCommandText()
        {
            var stringBuilder = new StringBuilder();

            var pkeySelectForInsert = new StringBuilder();
            var pkeyISelectForInsert = new StringBuilder();
            var pkeyAliasSelectForInsert = new StringBuilder();
            var pkeysLeftJoinForInsert = new StringBuilder();
            var pkeysIsNullForInsert = new StringBuilder();

            string and = string.Empty;
            string comma = string.Empty;
            foreach (var pkColumn in TableDescription.GetPrimaryKeysColumns())
            {
                var columnName = ParserName.Parse(pkColumn).Quoted().ToString();
                var parameterName = ParserName.Parse(pkColumn).Unquoted().Normalized().ToString();

                pkeySelectForInsert.Append($"{comma}{columnName}");
                pkeyISelectForInsert.Append($"{comma}[i].{columnName}");
                pkeyAliasSelectForInsert.Append($"{comma}@{parameterName} as {columnName}");
                pkeysLeftJoinForInsert.Append($"{and}[side].{columnName} = [i].{columnName}");
                pkeysIsNullForInsert.Append($"{and}[side].{columnName} IS NULL");
                and = " AND ";
                comma = ", ";
            }

            stringBuilder.AppendLine($"INSERT OR REPLACE INTO {trackingName.Schema().Quoted().ToString()} (");
            stringBuilder.AppendLine(pkeySelectForInsert.ToString());
            stringBuilder.AppendLine(",[update_scope_id], [sync_row_is_tombstone], [timestamp], [last_change_datetime] )");
            stringBuilder.AppendLine($"SELECT {pkeyISelectForInsert.ToString()} ");
            stringBuilder.AppendLine($"   , i.sync_scope_id, i.sync_row_is_tombstone, i.sync_timestamp, i.UtcDate");
            stringBuilder.AppendLine("FROM (");
            stringBuilder.AppendLine($"  SELECT {pkeyAliasSelectForInsert}");
            stringBuilder.AppendLine($"          ,@sync_scope_id as sync_scope_id, @sync_row_is_tombstone as sync_row_is_tombstone, {SqliteObjectNames.TimestampValue} as sync_timestamp, datetime('now') as UtcDate) as i;");


            var cmdtext = stringBuilder.ToString();

            AddCommandName(DbCommandType.UpdateMetadata, cmdtext);
        }

        private void CreateInitializeCommandText()
        {
            var stringBuilderArguments = new StringBuilder();
            var stringBuilderParameters = new StringBuilder();
            var stringBuilderParametersValues = new StringBuilder();
            var stringBuilderParametersValues2 = new StringBuilder();
            string empty = string.Empty;

            string str1 = SqliteManagementUtils.JoinOneTablesOnParametersValues(TableDescription.PrimaryKeys, "[side]");
            string str2 = SqliteManagementUtils.JoinOneTablesOnParametersValues(TableDescription.PrimaryKeys, "[base]");
            string str7 = SqliteManagementUtils.JoinTwoTablesOnClause(TableDescription.PrimaryKeys, "[p]", "[side]");

            // Generate Update command
            var stringBuilder = new StringBuilder();

            foreach (var mutableColumn in TableDescription.GetMutableColumns(false, true))
            {
                var columnName = ParserName.Parse(mutableColumn).Quoted().ToString();
                var columnParameterName = ParserName.Parse(mutableColumn).Unquoted().Normalized().ToString();
                stringBuilderParametersValues.Append($"{empty}@{columnParameterName} as {columnName}");
                stringBuilderParametersValues2.Append($"{empty}@{columnParameterName}");
                stringBuilderArguments.Append($"{empty}{columnName}");
                stringBuilderParameters.Append($"{empty}[c].{columnName}");
                empty = ", ";
            }

            stringBuilder.AppendLine($"INSERT OR IGNORE INTO {tableName.Quoted().ToString()}");
            stringBuilder.AppendLine($"({stringBuilderArguments.ToString()})");
            stringBuilder.Append($"VALUES ({stringBuilderParametersValues2.ToString()}) ");
            stringBuilder.AppendLine($";");

            stringBuilder.AppendLine($"UPDATE OR IGNORE {trackingName.Quoted().ToString()} SET ");
            stringBuilder.AppendLine("[update_scope_id] = @sync_scope_id,");
            stringBuilder.AppendLine("[sync_row_is_tombstone] = 0,");
            stringBuilder.AppendLine("[last_change_datetime] = datetime('now')");
            stringBuilder.AppendLine($"WHERE {SqliteManagementUtils.WhereColumnAndParameters(TableDescription.PrimaryKeys, "")}");
            stringBuilder.Append($" AND (select changes()) > 0");
            stringBuilder.AppendLine($";");
            var cmdtext = stringBuilder.ToString();

            AddCommandName(DbCommandType.InsertRow, cmdtext);
            AddCommandName(DbCommandType.InsertRows, cmdtext);
        }

        private void CreateUpdateCommandText()
        {
            var stringBuilderArguments = new StringBuilder();
            var stringBuilderParameters = new StringBuilder();
            var stringBuilderParametersValues = new StringBuilder();
            string empty = string.Empty;

            string str1 = SqliteManagementUtils.JoinOneTablesOnParametersValues(TableDescription.PrimaryKeys, "[side]");
            string str2 = SqliteManagementUtils.JoinOneTablesOnParametersValues(TableDescription.PrimaryKeys, "[base]");

            // Generate Update command
            var stringBuilder = new StringBuilder();

            foreach (var mutableColumn in TableDescription.GetMutableColumns(false, true))
            {
                var columnName = ParserName.Parse(mutableColumn).Quoted().ToString();
                var columnParameterName = ParserName.Parse(mutableColumn).Unquoted().Normalized().ToString();

                stringBuilderParametersValues.Append($"{empty}@{columnParameterName} as {columnName}");
                stringBuilderArguments.Append($"{empty}{columnName}");
                stringBuilderParameters.Append($"{empty}[c].{columnName}");
                empty = "\n, ";
            }

            // create update statement without PK
            var emptyUpdate = string.Empty;
            var columnsToUpdate = false;
            var stringBuilderUpdateSet = new StringBuilder();
            foreach (var mutableColumn in TableDescription.GetMutableColumns(false, false))
            {
                var columnName = ParserName.Parse(mutableColumn).Quoted().ToString();
                stringBuilderUpdateSet.Append($"{emptyUpdate}{columnName}=excluded.{columnName}");
                emptyUpdate = "\n, ";

                columnsToUpdate = true;
            }

            var primaryKeys = string.Join(",",
                TableDescription.PrimaryKeys.Select(name => ParserName.Parse(name).Quoted().ToString()));

            // add CTE
            stringBuilder.AppendLine($"WITH CHANGESET as (SELECT {stringBuilderParameters.ToString()} ");
            stringBuilder.AppendLine($"FROM (SELECT {stringBuilderParametersValues.ToString()}) as [c]");
            stringBuilder.AppendLine($"LEFT JOIN {trackingName.Quoted().ToString()} AS [side] ON {str1}");
            stringBuilder.AppendLine($"LEFT JOIN {tableName.Quoted().ToString()} AS [base] ON {str2}");
            //stringBuilder.AppendLine($"WHERE ({SqliteManagementUtils.WhereColumnAndParameters(TableDescription.PrimaryKeys, "[base]")} ");
            stringBuilder.AppendLine($"WHERE ([side].[timestamp] < @sync_min_timestamp OR [side].[update_scope_id] = @sync_scope_id) ");
            stringBuilder.Append($"OR ({SqliteManagementUtils.WhereColumnIsNull(TableDescription.PrimaryKeys, "[base]")} ");
            stringBuilder.AppendLine($"AND ([side].[timestamp] < @sync_min_timestamp OR [side].[timestamp] IS NULL)) ");
            stringBuilder.Append($"OR @sync_force_write = 1");
            stringBuilder.AppendLine($")");

            stringBuilder.AppendLine($"INSERT INTO {tableName.Quoted().ToString()}");
            stringBuilder.AppendLine($"({stringBuilderArguments.ToString()})");
            // use CTE here. The CTE is required in order to make the "ON CONFLICT" statement work. Otherwise SQLite cannot parse it
            // Note, that we have to add the pseudo WHERE TRUE clause here, as otherwise the SQLite parser may confuse the following ON
            // with a join clause, thus, throwing a parsing error
            // See a detailed explanation here at the official SQLite documentation: "Parsing Ambiguity" on page https://www.sqlite.org/lang_UPSERT.html
            stringBuilder.AppendLine($" SELECT * from CHANGESET WHERE TRUE");
            if (columnsToUpdate)
            {
                stringBuilder.AppendLine($" ON CONFLICT ({primaryKeys}) DO UPDATE SET ");
                stringBuilder.Append(stringBuilderUpdateSet.ToString()).AppendLine(";");
            }
            else
                stringBuilder.AppendLine($" ON CONFLICT ({primaryKeys}) DO NOTHING; ");

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"UPDATE OR IGNORE {trackingName.Quoted().ToString()} SET ");
            stringBuilder.AppendLine($"[update_scope_id] = @sync_scope_id,");
            stringBuilder.AppendLine($"[sync_row_is_tombstone] = 0,");
            stringBuilder.AppendLine($"[timestamp] = {SqliteObjectNames.TimestampValue},");
            stringBuilder.AppendLine($"[last_change_datetime] = datetime('now')");
            stringBuilder.AppendLine($"WHERE {SqliteManagementUtils.WhereColumnAndParameters(TableDescription.PrimaryKeys, "")}");
            stringBuilder.AppendLine($" AND (select changes()) > 0;");

            var cmdtext = stringBuilder.ToString();

            AddCommandName(DbCommandType.UpdateRow, cmdtext);
            AddCommandName(DbCommandType.UpdateRows, cmdtext);
        }

        //private void CreateUpdateCommandTextBack(bool hasMutableColumns)
        //{
        //    var stringBuilderArguments = new StringBuilder();
        //    var stringBuilderParameters = new StringBuilder();
        //    string empty = string.Empty;

        //    string str1 = SqliteManagementUtils.JoinTwoTablesOnClause(TableDescription.PrimaryKeys, "[c]", "[base]");
        //    string str7 = SqliteManagementUtils.JoinTwoTablesOnClause(TableDescription.PrimaryKeys, "[p]", "[side]");

        //    // Generate Update command
        //    var stringBuilder = new StringBuilder();


        //    stringBuilder.AppendLine(";WITH [c] AS (");
        //    stringBuilder.Append("\tSELECT ");
        //    foreach (var c in TableDescription.Columns.Where(col => !col.IsReadOnly))
        //    {
        //        var columnName = ParserName.Parse(c).Quoted().ToString();
        //        stringBuilder.Append($"[p].{columnName}, ");
        //    }
        //    stringBuilder.AppendLine();
        //    stringBuilder.AppendLine($"\t[side].[update_scope_id], [side].[timestamp], [side].[sync_row_is_tombstone]");
        //    stringBuilder.AppendLine($"\tFROM (SELECT ");
        //    stringBuilder.Append($"\t\t ");
        //    string comma = "";
        //    foreach (var c in TableDescription.GetMutableColumns(false, true))
        //    {
        //        var columnName = ParserName.Parse(c).Quoted().ToString();
        //        var columnParameterName = ParserName.Parse(c).Unquoted().Normalized().ToString();

        //        stringBuilder.Append($"{comma}@{columnParameterName} as {columnName}");
        //        comma = ", ";
        //    }
        //    stringBuilder.AppendLine($") AS [p]");
        //    stringBuilder.Append($"\tLEFT JOIN {trackingName.Quoted().ToString()} [side] ON ");
        //    stringBuilder.AppendLine($"\t{str7}");
        //    stringBuilder.AppendLine($"\t)");


        //    foreach (var mutableColumn in TableDescription.GetMutableColumns(false, true))
        //    {
        //        var columnName = ParserName.Parse(mutableColumn).Quoted().ToString();
        //        stringBuilderArguments.Append($"{empty}{columnName}");
        //        stringBuilderParameters.Append($"{empty}[c].{columnName}");
        //        empty = ", ";
        //    }

        //    stringBuilder.AppendLine($"INSERT OR REPLACE INTO {tableName.Quoted().ToString()}");
        //    stringBuilder.AppendLine($"({stringBuilderArguments.ToString()})");
        //    stringBuilder.AppendLine($"SELECT {stringBuilderParameters.ToString()} ");
        //    stringBuilder.AppendLine($"FROM [c] ");
        //    stringBuilder.AppendLine($"LEFT JOIN {tableName.Quoted().ToString()} AS [base] ON {str1}");
        //    stringBuilder.Append($"WHERE ({SqliteManagementUtils.WhereColumnAndParameters(TableDescription.PrimaryKeys, "[base]")} ");
        //    stringBuilder.AppendLine($"AND ([c].[timestamp] < @sync_min_timestamp OR [c].[update_scope_id] = @sync_scope_id)) ");
        //    stringBuilder.Append($"OR ({SqliteManagementUtils.WhereColumnIsNull(TableDescription.PrimaryKeys, "[base]")} ");
        //    stringBuilder.AppendLine($"AND ([c].[timestamp] < @sync_min_timestamp OR [c].[timestamp] IS NULL)) ");
        //    stringBuilder.AppendLine($"OR @sync_force_write = 1;");
        //    stringBuilder.AppendLine();
        //    stringBuilder.AppendLine($"UPDATE OR IGNORE {trackingName.Quoted().ToString()} SET ");
        //    stringBuilder.AppendLine("[update_scope_id] = @sync_scope_id,");
        //    stringBuilder.AppendLine("[sync_row_is_tombstone] = 0,");
        //    stringBuilder.AppendLine("[last_change_datetime] = datetime('now')");
        //    stringBuilder.AppendLine($"WHERE {SqliteManagementUtils.WhereColumnAndParameters(TableDescription.PrimaryKeys, "")}");
        //    stringBuilder.AppendLine($" AND (select changes()) > 0;");

        //    var cmdtext = stringBuilder.ToString();

        //    AddName(DbCommandType.UpdateRow, cmdtext);
        //}


        private void CreateDeleteMetadataCommandText()
        {

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"DELETE FROM {trackingName.Quoted().ToString()} WHERE [timestamp] < @sync_row_timestamp;");

            AddCommandName(DbCommandType.DeleteMetadata, stringBuilder.ToString());
        }
        private void CreateDeleteCommandText()
        {
            var stringBuilder = new StringBuilder();
            string str1 = SqliteManagementUtils.JoinTwoTablesOnClause(TableDescription.PrimaryKeys, "[c]", "[base]");
            string str7 = SqliteManagementUtils.JoinTwoTablesOnClause(TableDescription.PrimaryKeys, "[p]", "[side]");

            stringBuilder.AppendLine(";WITH [c] AS (");
            stringBuilder.Append("\tSELECT ");
            foreach (var c in TableDescription.GetPrimaryKeysColumns())
            {
                var columnName = ParserName.Parse(c).Quoted().ToString();
                stringBuilder.Append($"[p].{columnName}, ");
            }
            stringBuilder.AppendLine($"[side].[update_scope_id] as [sync_update_scope_id], [side].[timestamp] as [sync_timestamp], [side].[sync_row_is_tombstone]");
            stringBuilder.Append($"\tFROM (SELECT ");
            string comma = "";
            foreach (var c in TableDescription.GetPrimaryKeysColumns())
            {
                var columnName = ParserName.Parse(c).Quoted().ToString();
                var columnParameterName = ParserName.Parse(c).Unquoted().Normalized().ToString();

                stringBuilder.Append($"{comma}@{columnParameterName} as {columnName}");
                comma = ", ";
            }
            stringBuilder.AppendLine($") AS [p]");
            stringBuilder.Append($"\tLEFT JOIN {trackingName.Quoted().ToString()} [side] ON ");
            stringBuilder.AppendLine($"\t{str7}");
            stringBuilder.AppendLine($"\t)");

            stringBuilder.AppendLine($"DELETE FROM {tableName.Quoted().ToString()} ");
            stringBuilder.AppendLine($"WHERE {SqliteManagementUtils.WhereColumnAndParameters(TableDescription.PrimaryKeys, "")}");
            stringBuilder.AppendLine($"AND (EXISTS (");
            stringBuilder.AppendLine($"     SELECT * FROM [c] ");
            stringBuilder.AppendLine($"     WHERE {SqliteManagementUtils.WhereColumnAndParameters(TableDescription.PrimaryKeys, "[c]")}");
            stringBuilder.AppendLine($"     AND ([sync_timestamp] < @sync_min_timestamp OR [sync_timestamp] IS NULL OR [sync_update_scope_id] = @sync_scope_id))");
            stringBuilder.AppendLine($"  OR @sync_force_write = 1");
            stringBuilder.AppendLine($" );");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"UPDATE OR IGNORE {trackingName.Quoted().ToString()} SET ");
            stringBuilder.AppendLine("[update_scope_id] = @sync_scope_id,");
            stringBuilder.AppendLine("[sync_row_is_tombstone] = 1,");
            stringBuilder.AppendLine("[last_change_datetime] = datetime('now')");
            stringBuilder.AppendLine($"WHERE {SqliteManagementUtils.WhereColumnAndParameters(TableDescription.PrimaryKeys, "")}");
            stringBuilder.AppendLine($" AND (select changes()) > 0");

            var cmdText = stringBuilder.ToString();

            AddCommandName(DbCommandType.DeleteRow, cmdText);
            AddCommandName(DbCommandType.DeleteRows, cmdText);
        }
        private void CreateSelectRowCommandText()
        {
            StringBuilder stringBuilder = new StringBuilder("SELECT ");
            stringBuilder.AppendLine();
            StringBuilder stringBuilder1 = new StringBuilder();
            string empty = string.Empty;
            foreach (var pkColumn in TableDescription.GetPrimaryKeysColumns())
            {
                var columnName = ParserName.Parse(pkColumn).Quoted().ToString();
                var unquotedColumnName = ParserName.Parse(pkColumn).Unquoted().Normalized().ToString();
                stringBuilder.AppendLine($"\t[side].{columnName}, ");
                stringBuilder1.Append($"{empty}[side].{columnName} = @{unquotedColumnName}");
                empty = " AND ";
            }
            foreach (var mutableColumn in TableDescription.GetMutableColumns())
            {
                var nonPkColumnName = ParserName.Parse(mutableColumn).Quoted().ToString();
                stringBuilder.AppendLine($"\t[base].{nonPkColumnName}, ");
            }
            stringBuilder.AppendLine("\t[side].[sync_row_is_tombstone], ");
            stringBuilder.AppendLine("\t[side].[update_scope_id] as [sync_update_scope_id]");

            stringBuilder.AppendLine($"FROM {trackingName.Quoted().ToString()} [side] ");
            stringBuilder.AppendLine($"LEFT JOIN {tableName.Quoted().ToString()} [base] ON ");

            string str = string.Empty;
            foreach (var pkColumn in TableDescription.GetPrimaryKeysColumns())
            {
                var columnName = ParserName.Parse(pkColumn).Quoted().ToString();
                stringBuilder.Append($"{str}[base].{columnName} = [side].{columnName}");
                str = " AND ";
            }
            stringBuilder.AppendLine();
            stringBuilder.Append(string.Concat("WHERE ", stringBuilder1.ToString()));
            stringBuilder.Append(";");
            AddCommandName(DbCommandType.SelectRow, stringBuilder.ToString());
        }
        private void CreateSelectChangesCommandText()
        {
            var stringBuilder = new StringBuilder("SELECT ");
            foreach (var pkColumn in TableDescription.GetPrimaryKeysColumns())
            {
                var columnName = ParserName.Parse(pkColumn).Quoted().ToString();
                stringBuilder.AppendLine($"\t[side].{columnName}, ");
            }
            foreach (var mutableColumn in TableDescription.GetMutableColumns())
            {
                var columnName = ParserName.Parse(mutableColumn).Quoted().ToString();
                stringBuilder.AppendLine($"\t[base].{columnName}, ");
            }
            stringBuilder.AppendLine($"\t[side].[sync_row_is_tombstone], ");
            stringBuilder.AppendLine($"\t[side].[update_scope_id] as [sync_update_scope_id] ");
            stringBuilder.AppendLine($"FROM {trackingName.Quoted().ToString()} [side]");
            stringBuilder.AppendLine($"LEFT JOIN {tableName.Quoted().ToString()} [base]");
            stringBuilder.Append($"ON ");

            string empty = "";
            foreach (var pkColumn in TableDescription.GetPrimaryKeysColumns())
            {
                var columnName = ParserName.Parse(pkColumn).Quoted().ToString();

                stringBuilder.Append($"{empty}[base].{columnName} = [side].{columnName}");
                empty = " AND ";
            }
            stringBuilder.AppendLine();
            //stringBuilder.AppendLine("WHERE (");
            //stringBuilder.AppendLine("\t[side].[timestamp] > @sync_min_timestamp");
            //stringBuilder.AppendLine("\tAND ([side].[update_scope_id] <> @sync_scope_id OR [side].[update_scope_id] IS NULL)");
            //stringBuilder.AppendLine(")");

            // Looking at discussion https://github.com/Mimetis/Dotmim.Sync/discussions/453, trying to remove ([side].[update_scope_id] <> @sync_scope_id)
            // since we are sure that sqlite will never be a server side database

            stringBuilder.AppendLine("WHERE ([side].[timestamp] > @sync_min_timestamp AND [side].[update_scope_id] IS NULL)");


            AddCommandName(DbCommandType.SelectChanges, stringBuilder.ToString());
            AddCommandName(DbCommandType.SelectChangesWithFilters, stringBuilder.ToString());
        }


        private void CreateSelectInitializedCommandText()
        {
            StringBuilder stringBuilder = new StringBuilder("SELECT ");
            foreach (var pkColumn in TableDescription.GetPrimaryKeysColumns())
            {
                var columnName = ParserName.Parse(pkColumn).Quoted().ToString();
                stringBuilder.AppendLine($"\t[base].{columnName}, ");
            }
            var columns = TableDescription.GetMutableColumns().ToList();

            for (var i = 0; i < columns.Count; i++)
            {
                var mutableColumn = columns[i];
                var columnName = ParserName.Parse(mutableColumn).Quoted().ToString();
                stringBuilder.Append($"\t[base].{columnName}");

                if (i < columns.Count - 1)
                    stringBuilder.AppendLine(", ");
            }
            stringBuilder.AppendLine($"FROM {tableName.Quoted().ToString()} [base]");


            AddCommandName(DbCommandType.SelectInitializedChanges, stringBuilder.ToString());
            AddCommandName(DbCommandType.SelectInitializedChangesWithFilters, stringBuilder.ToString());
        }

        private void CreateUpdateUntrackedRowsCommandText()
        {
            var stringBuilder = new StringBuilder();
            var str1 = new StringBuilder();
            var str2 = new StringBuilder();
            var str3 = new StringBuilder();
            var str4 = SqliteManagementUtils.JoinTwoTablesOnClause(TableDescription.PrimaryKeys, "[side]", "[base]");

            stringBuilder.AppendLine($"INSERT INTO {trackingName.Schema().Quoted().ToString()} (");


            var comma = "";
            foreach (var pkeyColumn in TableDescription.GetPrimaryKeysColumns())
            {
                var pkeyColumnName = ParserName.Parse(pkeyColumn).Quoted().ToString();

                str1.Append($"{comma}{pkeyColumnName}");
                str2.Append($"{comma}[base].{pkeyColumnName}");
                str3.Append($"{comma}[side].{pkeyColumnName}");

                comma = ", ";
            }
            stringBuilder.Append(str1.ToString());
            stringBuilder.AppendLine($", [update_scope_id], [sync_row_is_tombstone], [timestamp], [last_change_datetime]");
            stringBuilder.AppendLine($")");
            stringBuilder.Append($"SELECT ");
            stringBuilder.Append(str2.ToString());
            stringBuilder.AppendLine($", NULL, 0, {SqliteObjectNames.TimestampValue}, datetime('now')");
            stringBuilder.AppendLine($"FROM {tableName.Schema().Quoted().ToString()} as [base] WHERE NOT EXISTS");
            stringBuilder.Append($"(SELECT ");
            stringBuilder.Append(str3.ToString());
            stringBuilder.AppendLine($" FROM {trackingName.Schema().Quoted().ToString()} as [side] ");
            stringBuilder.AppendLine($"WHERE {str4})");

            var r = stringBuilder.ToString();

            AddCommandName(DbCommandType.UpdateUntrackedRows, r);

        }

    }
}
