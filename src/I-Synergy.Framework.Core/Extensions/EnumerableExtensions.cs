using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ISynergy.Framework.Core.Exceptions;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class EnumerableExtensions.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines whether the two specified values have any flags in common.
        /// </summary>
        /// <param name="value">Value to test</param>
        /// <param name="desiredFlags">Flags we wish to find</param>
        /// <returns>Whether the two specified values have any flags in common.</returns>
        public static bool HasAny(this Enum value, Enum desiredFlags)
        {
            foreach (var flag in desiredFlags.GetIndividualFlags().EnsureNotNull())
            {
                if (value.HasFlag(flag))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Counts the pages of an <see cref="IEnumerable{T}" /> count result, according to a certain pagesize.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pageSize">Has to be greater than 0.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="ArgumentOutOfRangeException">pageSize - Value must be greater than 0.</exception>
        public static int CountPages<TSource>(this IEnumerable<TSource> source, int pageSize)
        {
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Value must be greater than 0.");

            return source.Count() / pageSize;
        }

        /// <summary>
        /// Applies paging to a <see cref="IEnumerable{T}" />. Take note that this should be applied after
        /// any Where-clauses, to make sure you're not missing any results.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="page">Has to be non-negative.</param>
        /// <param name="pageSize">Has to be greater than 0.</param>
        /// <returns>IEnumerable&lt;TSource&gt;.</returns>
        /// <exception cref="ArgumentBelowZeroException">pageSize</exception>
        /// <exception cref="ArgumentOutOfRangeException">pageSize - Value must be greater than 0.</exception>
        public static IEnumerable<TSource> ToPage<TSource>(this IEnumerable<TSource> source, int page, int pageSize = int.MaxValue)
        {
            if (page < 0)
                throw new ArgumentBelowZeroException(nameof(pageSize));
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Value must be greater than 0.");

            return source
                .Skip(page * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>IEnumerable&lt;Enum&gt;.</returns>
        public static IEnumerable<Enum> GetFlags(this Enum value)
        {
            return GetFlags(value, Enum.GetValues(value.GetType()).Cast<Enum>().ToArray());
        }

        /// <summary>
        /// Gets the individual flags.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>IEnumerable&lt;Enum&gt;.</returns>
        public static IEnumerable<Enum> GetIndividualFlags(this Enum value)
        {
            return GetFlags(value, GetFlagValues(value.GetType()).ToArray());
        }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">The values.</param>
        /// <returns>IEnumerable&lt;Enum&gt;.</returns>
        private static IEnumerable<Enum> GetFlags(Enum value, Enum[] values)
        {
            var bits = Convert.ToUInt64(value);
            var results = new List<Enum>();
            for (var i = values.Length - 1; i >= 0; i--)
            {
                var mask = Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;
                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }
            if (bits != 0L)
                return Enumerable.Empty<Enum>();
            if (Convert.ToUInt64(value) != 0L)
                return results.Reverse<Enum>();
            if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
                return values.Take(1);
            return Enumerable.Empty<Enum>();
        }

        /// <summary>
        /// Gets the flag values.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns>IEnumerable&lt;Enum&gt;.</returns>
        private static IEnumerable<Enum> GetFlagValues(Type enumType)
        {
            ulong flag = 0x1;
            foreach (var value in Enum.GetValues(enumType).Cast<Enum>().EnsureNotNull())
            {
                var bits = Convert.ToUInt64(value);
                if (bits == 0L)
                    //yield return value;
                    continue; // skip the zero value
                while (flag < bits) flag <<= 1;
                if (flag == bits)
                    yield return value;
            }
        }

        /// <summary>
        /// Gets the type of t.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_">The .</param>
        /// <returns>Type.</returns>
        public static Type GetTypeOfT<T>(this IEnumerable<T> _)
        {
            return typeof(T);
        }

        /// <summary>
        /// Converts to datatable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>DataTable.</returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection, string tableName)
        {
            var tbl = collection.ToDataTable();
            tbl.TableName = tableName;
            return tbl;
        }

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
