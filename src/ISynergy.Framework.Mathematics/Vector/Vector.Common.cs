namespace ISynergy.Framework.Mathematics;

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