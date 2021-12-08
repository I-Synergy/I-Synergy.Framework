using ISynergy.Framework.Synchronization.Core.Model.Parsers;
using ISynergy.Framework.Synchronization.SqlServer.Utilities;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Extensions
{
    internal static class DbConnectionExtensions
    {
        public static Task<DbCommand> ToCommandAsync(this DbConnection connection, DbTransaction transaction, ParserName tableName)
        {
            var commandText = $"IF EXISTS (Select top 1 tbl.name as TableName, " +
                              $"sch.name as SchemaName " +
                              $"  from sys.change_tracking_tables tr " +
                              $"  Inner join sys.tables as tbl on tbl.object_id = tr.object_id " +
                              $"  Inner join sys.schemas as sch on tbl.schema_id = sch.schema_id " +
                              $"  Where tbl.name = @tableName and sch.name = @schemaName) SELECT 1 ELSE SELECT 0;";

            var tbl = tableName.Unquoted().ToString();
            var schema = SqlManagementUtils.GetUnquotedSqlSchemaName(tableName);

            var command = connection.CreateCommand();

            command.Connection = connection;
            command.Transaction = transaction;
            command.CommandText = commandText;

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@tableName";
            parameter.Value = tbl;
            command.Parameters.Add(parameter);

            parameter = command.CreateParameter();
            parameter.ParameterName = "@schemaName";
            parameter.Value = schema;
            command.Parameters.Add(parameter);

            return Task.FromResult(command);
        }
    }
}
