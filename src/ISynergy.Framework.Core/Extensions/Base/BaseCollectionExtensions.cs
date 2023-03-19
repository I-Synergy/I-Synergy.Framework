using ISynergy.Framework.Core.Attributes;
using System.Collections;
using System.Data;
using System.Reflection;

namespace ISynergy.Framework.Core.Extensions.Base
{
    /// <summary>
    /// Base collection extension
    /// </summary>
    internal static class BaseCollectionExtensions
    {
        /// <summary>
        /// Converts to datatable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="name">The name.</param>
        /// <returns>DataTable.</returns>
        public static DataTable ToDataTableBase<T>(this IEnumerable<T> collection, string name) =>
            collection.ToDataTableBase(name, typeof(T));

        /// <summary>
        /// Converts to datatable.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns>DataTable.</returns>
        public static DataTable ToDataTableBase(this IEnumerable collection, string name, Type type)
        {
            var dataTable = new DataTable()
            {
                TableName = name
            };

            //Populate the table
            if (collection is not null)
            {
                // Exclude all properties that are have the JsonIgnoreAttribute or are an Enumarable.
                var typeProperties = type.GetProperties()
                    .Where(q => !q.IsDefined(typeof(DataTableIgnoreAttribute)) &&
                        (q.PropertyType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(q.PropertyType)))
                    .ToArray();

                foreach (var typeProperty in typeProperties)
                {
                    dataTable.Columns.Add(typeProperty.Name, Nullable.GetUnderlyingType(typeProperty.PropertyType) ?? typeProperty.PropertyType).AllowDBNull = true;
                }

                foreach (var item in collection)
                {
                    var dataRow = dataTable.NewRow();
                    dataRow.BeginEdit();

                    foreach (var typeProperty in typeProperties)
                    {
                        var temp = typeProperty.GetValue(item) ?? DBNull.Value;

                        if (temp is null || (temp.GetType().Name == "Char" && ((char)temp).Equals('\0')))
                        {
                            dataRow[typeProperty.Name] = DBNull.Value;
                        }
                        else
                        {
                            dataRow[typeProperty.Name] = temp;
                        }
                    }

                    dataRow.EndEdit();
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }
    }
}
