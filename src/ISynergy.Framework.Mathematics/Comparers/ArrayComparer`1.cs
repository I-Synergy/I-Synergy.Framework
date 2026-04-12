
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Comparers;

/// <summary>
///     Elementwise comparer for arrays.
/// </summary>
public class ArrayComparer<T> : IEqualityComparer<T[]>
    where T : IEquatable<T>
{
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
        for (var i = 0; i < x.Length; i++)
            if (!x[i].Equals(y[i]))
                return false;
        return true;
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
        unchecked
        {
            var hash = 17;
            for (var i = 0; i < obj.Length; i++)
                hash = hash * 23 + obj[i].GetHashCode();
            return hash;
        }
    }
}