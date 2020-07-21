using System;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Converters
{
    /// <summary>
    /// Class DataTableConverter.
    /// Implements the <see cref="JsonConverter{DataTable}" />
    /// </summary>
    /// <seealso cref="JsonConverter{DataTable}" />
    public partial class DataTableConverter : JsonConverter<DataTable>
    {
        /// <summary>
        /// Writes the data table json to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The reader.</param>
        /// <param name="options">The options.</param>
        public override void Write(Utf8JsonWriter writer, DataTable value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            if (value != null)
            {
                var dcolumns = value.Columns;
                var columnsCount = dcolumns.Count;
                var columnNames = new string[columnsCount];
                var columnTypes = new TypeCode[columnsCount];
                for (var columnIndex = 0; columnIndex < columnsCount; columnIndex++)
                {
                    var dc = dcolumns[columnIndex];
                    columnNames[columnIndex] = dc.ColumnName;
                    columnTypes[columnIndex] = Type.GetTypeCode(dc.DataType);
                }

                var drows = value.Rows;
                for (int rowIndex = 0, rowIndexMax = drows.Count; rowIndex < rowIndexMax; rowIndex++)
                {
                    var drow = drows[rowIndex].ItemArray;

                    writer.WriteStartObject();
                    for (var columnIndex = 0; columnIndex < columnsCount; columnIndex++)
                    {
                        var columnName = columnNames[columnIndex];
                        var cellValue = drow[columnIndex];

                        if (cellValue == DBNull.Value)
                        {
                            writer.WriteNull(columnName);
                        }
                        else
                        {
                            var valueType = columnTypes[columnIndex];
                            switch (valueType)
                            {
                                case TypeCode.Boolean:
                                    {
                                        writer.WriteBoolean(columnName, (bool)cellValue);
                                        break;
                                    }
                                case TypeCode.Byte:
                                    {
                                        writer.WriteNumber(columnName, (byte)cellValue);
                                        break;
                                    }
                                case TypeCode.Char:
                                    {
                                        writer.WriteNumber(columnName, (char)cellValue);
                                        break;
                                    }
                                case TypeCode.Int32:
                                    {
                                        writer.WriteNumber(columnName, (int)cellValue);
                                        break;
                                    }
                                case TypeCode.Int16:
                                    {
                                        writer.WriteNumber(columnName, (short)cellValue);
                                        break;
                                    }
                                case TypeCode.Int64:
                                    {
                                        writer.WriteNumber(columnName, (long)cellValue);
                                        break;
                                    }
                                case TypeCode.Single:
                                    {
                                        writer.WriteNumber(columnName, (float)cellValue);
                                        break;
                                    }
                                case TypeCode.Double:
                                    {
                                        writer.WriteNumber(columnName, (double)cellValue);
                                        break;
                                    }
                                case TypeCode.Decimal:
                                    {
                                        writer.WriteNumber(columnName, (decimal)cellValue);
                                        break;
                                    }
                                case TypeCode.DateTime:
                                    {
                                        writer.WriteString(columnName, (DateTime)cellValue);
                                        break;
                                    }
                                case TypeCode.String:
                                    {
                                        writer.WriteString(columnName, (string)cellValue);
                                        break;
                                    }
                                default:
                                    {
                                        writer.WriteString(columnName, Convert.ToString(cellValue));
                                        break;
                                    }
                            }
                        }
                    }
                    writer.WriteEndObject();
                }
            }

            writer.WriteEndArray();
        }

        /// <summary>
        /// Read override - not supported.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="type">The type.</param>
        /// <param name="options">The options.</param>
        /// <returns>DataTable.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public override DataTable Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
