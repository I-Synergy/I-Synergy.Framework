using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <returns>DataTable.</returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            var dt = new DataTable();
            var t = typeof(T);
            var pia = t.GetProperties();
            object temp;
            DataRow dr;

            for (var i = 0; i < pia.Length; i++)
            {
                dt.Columns.Add(pia[i].Name, Nullable.GetUnderlyingType(pia[i].PropertyType) ?? pia[i].PropertyType);
                dt.Columns[i].AllowDBNull = true;
            }

            //Populate the table
            foreach (var item in collection.EnsureNotNull())
            {
                dr = dt.NewRow();
                dr.BeginEdit();

                for (var i = 0; i < pia.Length; i++)
                {
                    temp = pia[i].GetValue(item) ?? DBNull.Value;

                    if (temp is null || (temp.GetType().Name == "Char" && ((char)temp).Equals('\0')))
                    {
                        dr[pia[i].Name] = DBNull.Value;
                    }
                    else
                    {
                        dr[pia[i].Name] = temp;
                    }
                }

                dr.EndEdit();
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}
