
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Comparers;

/// <summary>
///     Element-at-position comparer.
/// </summary>
/// <remarks>
///     This class compares arrays by checking the value
///     of a particular element at a given array index.
/// </remarks>
/// <example>
///     <code>
///   // We sort the arrays according to the 
///   // elements at their second column.
///   
///   double[][] values =
///   {   //                 v
///       new double[] {  0, 3, 0 },
///       new double[] {  0, 4, 1 },
///       new double[] { -1, 1, 1 },
///       new double[] { -1, 5, 4 },
///       new double[] { -2, 2, 6 },
///   };
///   
///   // Sort the array considering only the second column
///   Array.Sort(values, new ElementComparer() { Index = 1 });
///   
///   // The result will be
///   double[][] result =
///   {
///       new double[] { -1, 1, 1 },
///       new double[] { -2, 2, 6 },
///       new double[] {  0, 3, 0 },
///       new double[] {  0, 4, 1 },
///       new double[] { -1, 5, 4 },
///   };
/// </code>
/// </example>
/// <seealso cref="ElementComparer{T}" />
/// <seealso cref="ArrayComparer{T}" />
/// <seealso cref="GeneralComparer" />
/// <seealso cref="CustomComparer{T}" />
public class ElementComparer : ElementComparer<double>
{
}

/// <summary>
///     Element-at-position comparer.
/// </summary>
/// <remarks>
///     This class compares arrays by checking the value
///     of a particular element at a given array index.
/// </remarks>
/// <example>
///     <code>
///   // We sort the arrays according to the 
///   // elements at their second column.
///   
///   double[][] values =
///   {   //                 v
///       new double[] {  0, 3, 0 },
///       new double[] {  0, 4, 1 },
///       new double[] { -1, 1, 1 },
///       new double[] { -1, 5, 4 },
///       new double[] { -2, 2, 6 },
///   };
///   
///   // Sort the array considering only the second column
///   Array.Sort(values, new ElementComparer() { Index = 1 });
///   
///   // The result will be
///   double[][] result =
///   {
///       new double[] { -1, 1, 1 },
///       new double[] { -2, 2, 6 },
///       new double[] {  0, 3, 0 },
///       new double[] {  0, 4, 1 },
///       new double[] { -1, 5, 4 },
///   };
/// </code>
/// </example>
/// <seealso cref="ElementComparer" />
/// <seealso cref="ArrayComparer{T}" />
/// <seealso cref="GeneralComparer" />
/// <seealso cref="CustomComparer{T}" />
public class ElementComparer<T> : IComparer<T[]>, IEqualityComparer<T[]>
    where T : IComparable, IEquatable<T>
{
    /// <summary>
    ///     Gets or sets the element index to compare.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    ///     Compares two objects and returns a value indicating
    ///     whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    public int Compare(T[] x, T[] y)
    {
        return x[Index].CompareTo(y[Index]);
    }

    /// <summary>
    ///     Determines whether two instances are equal.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if the specified object is equal to the other; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(T[] x, T[] y)
    {
        return x[Index].Equals(y[Index]);
    }

    /// <summary>
    ///     Returns a hash code for a given instance.
    /// </summary>
    /// <param name="obj">The instance.</param>
    /// <returns>
    ///     A hash code for the instance, suitable for use
    ///     in hashing algorithms and data structures like a hash table.
    /// </returns>
    public int GetHashCode(T[] obj)
    {
        return obj[Index].GetHashCode();
    }
}