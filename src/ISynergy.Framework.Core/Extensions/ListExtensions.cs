using ISynergy.Framework.Core.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="IList" /> and <see cref="IList{T}" /> classes.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Ensures list is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns>IList&lt;T&gt;.</returns>
        public static IList<T> EnsureNotNull<T>(this IList<T> list)
        {
            return list ?? new List<T>();
        }

        /// <summary>
        /// Ensures enumerable is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> EnsureNotNull<T>(this IEnumerable<T> list)
        {
            return list ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Ensures observablecollection is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns>ObservableConcurrentCollection&lt;T&gt;.</returns>
        public static ObservableConcurrentCollection<T> EnsureNotNull<T>(this ObservableConcurrentCollection<T> list)
        {
            return list ?? new ObservableConcurrentCollection<T>();
        }

        /// <summary>
        /// Gets the type of t.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_">The .</param>
        /// <returns>Type.</returns>
        public static Type GetTypeOfT<T>(this IList<T> _)
        {
            return typeof(T);
        }
    }
}
