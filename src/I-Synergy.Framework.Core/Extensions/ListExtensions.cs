using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace System.Collections
{
    /// <summary>
    /// Extensions for the <see cref="IList"/> and <see cref="IList{T}"/> classes.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Ensures list is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList<T> EnsureNotNull<T>(this IList<T> list)
        {
            return list ?? new List<T>();
        }

        /// <summary>
        /// Ensures enumerable is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> EnsureNotNull<T>(this IEnumerable<T> list)
        {
            return list ??  Enumerable.Empty<T>();
        }

        /// <summary>
        /// Ensures observablecollection is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ObservableCollection<T> EnsureNotNull<T>(this ObservableCollection<T> list)
        {
            return list ?? new ObservableCollection<T>();
        }
    }
}
