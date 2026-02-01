namespace ISynergy.Framework.Mathematics.Comparers;

/// <summary>
///     Stable comparer for stable sorting algorithm.
/// </summary>
/// <typeparam name="T">The type of objects to compare.</typeparam>
/// <remarks>
///     This class helps sort the elements of an array without swapping
///     elements which are already in order. This comprises a <c>stable</c>
///     sorting algorithm.
/// </remarks>
/// <example>
/// </example>
/// <seealso cref="ElementComparer{T}" />
/// <seealso cref="ArrayComparer{T}" />
/// <seealso cref="GeneralComparer" />
/// <seealso cref="CustomComparer{T}" />
public class StableComparer<T> : IComparer<KeyValuePair<int, T>>
{
    private readonly Comparison<T> comparison;

    /// <summary>
    ///     Constructs a new instance of the <see cref="StableComparer{T}" /> class.
    /// </summary>
    /// <param name="comparison">The comparison function.</param>
    public StableComparer(Comparison<T> comparison)
    {
        this.comparison = comparison;
    }

    /// <summary>
    ///     Compares two objects and returns a value indicating
    ///     whether one is less than, equal to, or greater than
    ///     the other.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>A signed integer that indicates the relative values of x and y.</returns>
    public int Compare(KeyValuePair<int, T> x, KeyValuePair<int, T> y)
    {
        var result = comparison(x.Value, y.Value);
        return result != 0 ? result : x.Key.CompareTo(y.Key);
    }
}