using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ISynergy.Framework.Core.Exceptions;

namespace ISynergy.Framework.Core.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines whether the two specified values have any flags in common.
        /// </summary>
        /// <param name="value">Value to test</param>
        /// <param name="desiredFlags">Flags we wish to find</param>
        /// <returns>Whether the two specified values have any flags in common.</returns>
        /// <exception cref="TypeArgumentException"><typeparamref name="T"/> is not a flags enum.</exception>
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
        /// Counts the pages of an <see cref="IEnumerable{T}"/> count result, according to a certain pagesize.
        /// </summary>
        /// <param name="pageSize">Has to be greater than 0.</param>
        public static int CountPages<TSource>(this IEnumerable<TSource> source, int pageSize)
        {
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Value must be greater than 0.");

            return source.Count() / pageSize;
        }

        /// <summary>
        /// Applies paging to a <see cref="IEnumerable{T}"/>. Take note that this should be applied after
        /// any Where-clauses, to make sure you're not missing any results.
        /// </summary>
        /// <param name="page">Has to be non-negative.</param>
        /// <param name="pageSize">Has to be greater than 0.</param>
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

        public static IEnumerable<Enum> GetFlags(this Enum value)
        {
            return GetFlags(value, Enum.GetValues(value.GetType()).Cast<Enum>().ToArray());
        }

        public static IEnumerable<Enum> GetIndividualFlags(this Enum value)
        {
            return GetFlags(value, GetFlagValues(value.GetType()).ToArray());
        }

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

        public static Type GetTypeOfT<T>(this IEnumerable<T> _)
        {
            return typeof(T);
        }

#if !WINDOWS_UWP

        public static DataTable ToDataTable<T>(this IEnumerable<T> collection, string tableName)
        {
            var tbl = ToDataTable(collection);
            tbl.TableName = tableName;
            return tbl;
        }

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
                        dr[pia[i].Name] = (object)DBNull.Value;
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

#endif
    }
}
