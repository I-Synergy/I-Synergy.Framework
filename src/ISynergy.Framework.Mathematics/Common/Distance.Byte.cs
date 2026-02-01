using ISynergy.Framework.Core.Extensions;
using System.Collections;

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
