using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Core.Extensions;

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
    [return: NotNull]
    public static IList<T> EnsureNotNull<T>([AllowNull] this IList<T>? list)
    {
        if (list is null)
            list = new List<T>();

        return list;
    }

    /// <summary>
    /// Ensures enumerable is not null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list.</param>
    /// <returns>IEnumerable&lt;T&gt;.</returns>
    [return: NotNull]
    public static IEnumerable<T> EnsureNotNull<T>([AllowNull] this IEnumerable<T>? list)
    {
        if (list is null)
            list = Enumerable.Empty<T>();

        return list;
    }

    /// <summary>
    /// Ensures enumerable is not null
    /// </summary>
    /// <param name="list">The list.</param>
    /// <returns>IEnumerable.</returns>
    [return: NotNull]
    public static IEnumerable EnsureNotNull([AllowNull] this IEnumerable? list)
    {
        if (list is null)
            list = Enumerable.Empty<object>();

        return list;
    }

    /// <summary>
    /// Ensures observablecollection is not null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list.</param>
    /// <returns>ObservableCollection&lt;T&gt;.</returns>
    [return: NotNull]
    public static ObservableCollection<T> EnsureNotNull<T>([AllowNull] this ObservableCollection<T>? list)
    {
        if (list is null)
            list = new ObservableCollection<T>();

        return list;
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
