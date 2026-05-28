using ISynergy.Framework.Mathematics.Matrices;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.Vectors;

public static partial class Vector
{
    /// <summary>
    ///     Creates a vector containing every index that can be used to
    ///     address a given <paramref name="array" />, in order.
    /// </summary>
    /// <param name="array">The array whose indices will be returned.</param>
    /// <returns>
    ///     A vector of the same size as the given <paramref name="array" />
    ///     containing all vector indices from 0 up to the length of
    ///     <paramref name="array" />.
    /// </returns>
    /// <example>
    ///     <code>
    ///   double[] a = { 5.3, 2.3, 4.2 };
    ///   int[] idx = a.GetIndices(); // output will be { 0, 1, 2 }
    /// </code>
    /// </example>
    /// <seealso cref="Matrix.GetIndices" />
    public static int[] GetIndices<T>(this T[] array)
    {
        return Range(0, array.Length);
    }
}