using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Data
{
    public static class DataReaderExtensions
    {
        public static List<T> MapToList<T>(this IDataReader datareader)
        {
            var list = new List<T>();
            T obj = default;

            while (datareader.Read())
            {
                var fieldNames = Enumerable.Range(0, datareader.FieldCount).Select(i => datareader.GetName(i)).ToArray();
                obj = Activator.CreateInstance<T>();

                foreach (var prop in obj.GetType().GetProperties().EnsureNotNull())
                {
                    if (fieldNames.Contains(prop.Name) && !datareader.IsDBNull(prop.Name))
                    {
                        if (prop.PropertyType.GetTypeInfo().IsEnum)
                        {
                            prop.SetValue(obj, datareader[prop.Name]);
                        }
                        else
                        {
                            prop.SetValue(obj, Convert.ChangeType(datareader[prop.Name], prop.PropertyType), null);
                        }
                    }
                }

                list.Add(obj);
            }

            return list;
        }

        /// <summary>
        /// Checks if a column's value is DBNull
        /// </summary>
        /// <param name="dataReader">The data reader</param>
        /// <param name="columnName">The column name</param>
        /// <returns>A bool indicating if the column's value is DBNull</returns>
        public static bool IsDBNull(this IDataReader dataReader, string columnName)
        {
            return dataReader[columnName] == DBNull.Value;
        }
    }
}