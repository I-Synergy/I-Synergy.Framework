using ISynergy.Framework.Core.Extensions;
using System.Collections;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.Common;

public static partial class Distance
{
    /// <summary>
    ///   Gets the Hamming distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Hamming distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Hamming"/> documentation page.
    /// </example>
    ///
    public static double Hamming(byte[] x, byte[] y) => Hamming(x.ToDoubleArray(), y.ToDoubleArray());

    /// <summary>
    ///   Gets the Hamming distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Hamming distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Hamming"/> documentation page.
    /// </example>
    ///
    public static double Hamming(BitArray x, BitArray y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheHamming.Distance(x, y);
    }
}
