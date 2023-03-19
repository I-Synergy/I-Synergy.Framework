using ISynergy.Framework.Core.Utilities;
using System.Data;
using System.Reflection;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class DataReaderExtensions.
    /// </summary>
    public static class DataReaderExtensions
    {
        /// <summary>
        /// Maps to list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datareader">The datareader.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public static List<T> MapToList<T>(this IDataReader datareader)
        {
            var list = new List<T>();

            while (datareader.Read())
            {
                var fieldNames = Enumerable.Range(0, datareader.FieldCount).Select(i => datareader.GetName(i)).ToArray();
                T obj = TypeActivator.CreateInstance<T>();

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
