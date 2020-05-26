using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq.Extensions
{
    /// <summary>
    /// Define extensions on <see cref="IEnumerable" />.
    /// </summary>
    public static class DynamicEnumerableExtensions
    {
        /// <summary>
        /// Converts to dynamicarraygenericmethod.
        /// </summary>
        private static readonly MethodInfo ToDynamicArrayGenericMethod;

        /// <summary>
        /// Initializes static members of the <see cref="DynamicEnumerableExtensions"/> class.
        /// </summary>
        static DynamicEnumerableExtensions()
        {
            ToDynamicArrayGenericMethod = typeof(DynamicEnumerableExtensions).GetTypeInfo().GetDeclaredMethods("ToDynamicArray")
                .First(x => x.IsGenericMethod);
        }

        /// <summary>
        /// Creates an array of dynamic objects from a <see cref="IEnumerable" />.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable" /> to create an array from.</param>
        /// <returns>An array that contains the elements from the input sequence.</returns>
        public static dynamic[] ToDynamicArray(this IEnumerable source)
        {
            Argument.IsNotNull(nameof(source), source);
            return CastToArray<dynamic>(source);
        }

        /// <summary>
        /// Creates an array of dynamic objects from a <see cref="IEnumerable" />.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="source">A <see cref="IEnumerable" /> to create an array from.</param>
        /// <returns>An Array{T} that contains the elements from the input sequence.</returns>
        public static T[] ToDynamicArray<T>(this IEnumerable source)
        {
            Argument.IsNotNull(nameof(source), source);
            return CastToArray<T>(source);
        }

        /// <summary>
        /// Creates an array of dynamic objects from a <see cref="IEnumerable" />.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable" /> to create an array from.</param>
        /// <param name="type">A <see cref="Type" /> cast to.</param>
        /// <returns>An Array that contains the elements from the input sequence.</returns>
        public static dynamic[] ToDynamicArray(this IEnumerable source, Type type)
        {
            Argument.IsNotNull(nameof(source), source);
            Argument.IsNotNull(nameof(type), type);

            IEnumerable result = (IEnumerable)ToDynamicArrayGenericMethod.MakeGenericMethod(type).Invoke(source, new object[] { source });
            return CastToArray<dynamic>(result);
        }

        /// <summary>
        /// Creates a list of dynamic objects from a <see cref="IEnumerable" />.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable" /> to create an array from.</param>
        /// <returns>A List that contains the elements from the input sequence.</returns>
        public static List<dynamic> ToDynamicList(this IEnumerable source)
        {
            Argument.IsNotNull(nameof(source), source);
            return CastToList<dynamic>(source);
        }

        /// <summary>
        /// Creates a list of dynamic objects from a <see cref="IEnumerable" />.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable" /> to create an array from.</param>
        /// <param name="type">A <see cref="Type" /> cast to.</param>
        /// <returns>A List that contains the elements from the input sequence.</returns>
        public static List<dynamic> ToDynamicList(this IEnumerable source, Type type)
        {
            Argument.IsNotNull(nameof(source), source);
            Argument.IsNotNull(nameof(type), type);

            return ToDynamicArray(source, type).ToList();
        }

        /// <summary>
        /// Creates a list of dynamic objects from a <see cref="IEnumerable" />.
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="source">A <see cref="IEnumerable" /> to create an array from.</param>
        /// <returns>A List{T} that contains the elements from the input sequence.</returns>
        public static List<T> ToDynamicList<T>(this IEnumerable source)
        {
            Argument.IsNotNull(nameof(source), source);
            return CastToList<T>(source);
        }

        /// <summary>
        /// Casts to array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>T[].</returns>
        internal static T[] CastToArray<T>(IEnumerable source)
        {
            return source.Cast<T>().ToArray();
        }

        /// <summary>
        /// Casts to list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal static List<T> CastToList<T>(IEnumerable source)
        {
            return source.Cast<T>().ToList();
        }
    }
}
