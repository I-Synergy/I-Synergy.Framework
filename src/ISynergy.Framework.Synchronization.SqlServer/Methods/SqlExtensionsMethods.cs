using ISynergy.Framework.Synchronization.Core.Model.Parsers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Methods
{
    public static class SqlExtensionsMethods
    {

        internal static async Task<SqlParameter[]> DeriveParametersAsync(this SqlConnection connection, SqlCommand cmd, bool includeReturnValueParameter = false, SqlTransaction transaction = null)
        {
            if (cmd is null) throw new ArgumentNullException("SqlCommand");

            var textParser = ParserName.Parse(cmd.CommandText);

            var schemaName = textParser.SchemaName;
            var spName = textParser.ObjectName;

            var alreadyOpened = connection.State == ConnectionState.Open;

            if (!alreadyOpened)
                await connection.OpenAsync().ConfigureAwait(false);

            try
            {
                var getParamsCommand = new SqlCommand("sp_procedure_params_rowset", connection);
                getParamsCommand.CommandType = CommandType.StoredProcedure;
                getParamsCommand.Transaction = transaction;

                var p = new SqlParameter("@procedure_name", SqlDbType.NVarChar);
                p.Value = spName;
                getParamsCommand.Parameters.Add(p);
                p = new SqlParameter("@procedure_schema", SqlDbType.NVarChar);
                p.Value = schemaName;
                getParamsCommand.Parameters.Add(p);

                using (var sdr = await getParamsCommand.ExecuteReaderAsync().ConfigureAwait(false))
                    // Do we have any rows?
                    if (sdr.HasRows)
                    {
                        // Read the parameter information
                        var ParamNameCol = sdr.GetOrdinal("PARAMETER_NAME");
                        var ParamSizeCol = sdr.GetOrdinal("CHARACTER_MAXIMUM_LENGTH");
                        var ParamTypeCol = sdr.GetOrdinal("TYPE_NAME");
                        var ParamNullCol = sdr.GetOrdinal("IS_NULLABLE");
                        var ParamPrecCol = sdr.GetOrdinal("NUMERIC_PRECISION");
                        var ParamDirCol = sdr.GetOrdinal("PARAMETER_TYPE");
                        var ParamScaleCol = sdr.GetOrdinal("NUMERIC_SCALE");

                        // Loop through and read the rows
                        while (sdr.Read())
                        {
                            var name = sdr.GetString(ParamNameCol);
                            var datatype = sdr.GetString(ParamTypeCol);

                            // Is this xml?
                            // ADO.NET 1.1 does not support XML, replace with text
                            //if (0 == String.Compare("xml", datatype, true))
                            //    datatype = "Text";

                            if (0 == string.Compare("table", datatype, true))
                                datatype = "Structured";

                            // TODO : Should we raise an error here ??
                            if (!Enum.TryParse(datatype, true, out SqlDbType type))
                                type = SqlDbType.Variant;

                            var Nullable = sdr.GetBoolean(ParamNullCol);
                            var param = new SqlParameter(name, type);

                            // Determine parameter direction
                            int dir = sdr.GetInt16(ParamDirCol);
                            switch (dir)
                            {
                                case 1:
                                    param.Direction = ParameterDirection.Input;
                                    break;
                                case 2:
                                    param.Direction = ParameterDirection.Output;
                                    break;
                                case 3:
                                    param.Direction = ParameterDirection.InputOutput;
                                    break;
                                case 4:
                                    param.Direction = ParameterDirection.ReturnValue;
                                    break;
                            }
                            param.IsNullable = Nullable;
                            if (!sdr.IsDBNull(ParamPrecCol))
                                param.Precision = (byte)sdr.GetInt16(ParamPrecCol);
                            if (!sdr.IsDBNull(ParamSizeCol))
                                param.Size = sdr.GetInt32(ParamSizeCol);
                            if (!sdr.IsDBNull(ParamScaleCol))
                                param.Scale = (byte)sdr.GetInt16(ParamScaleCol);

                            cmd.Parameters.Add(param);
                        }
                    }

            }
            finally
            {
                if (!alreadyOpened)
                    connection.Close();
            }

            if (!includeReturnValueParameter && cmd.Parameters.Count > 0)
                cmd.Parameters.RemoveAt(0);

            var discoveredParameters = new SqlParameter[cmd.Parameters.Count];

            cmd.Parameters.CopyTo(discoveredParameters, 0);

            // Init the parameters with a DBNull value
            foreach (var discoveredParameter in discoveredParameters)
                discoveredParameter.Value = DBNull.Value;

            return discoveredParameters;
        }



        internal static SqlParameter Clone(this SqlParameter param)
        {
            var p = new SqlParameter
            {
                DbType = param.DbType,
                Direction = param.Direction,
                IsNullable = param.IsNullable,
                ParameterName = param.ParameterName,
                Precision = param.Precision,
                Scale = param.Scale,
                Size = param.Size,
                SourceColumn = param.SourceColumn,
                SqlDbType = param.SqlDbType,
                SqlValue = param.SqlValue,
                TypeName = param.TypeName,
                Value = param.Value
            };

            return p;
        }

    }
}
