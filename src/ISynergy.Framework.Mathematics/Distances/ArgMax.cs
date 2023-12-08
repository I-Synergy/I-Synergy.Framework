namespace ISynergy.Framework.Mathematics.Distances;

using ISynergy.Framework.Mathematics.Distances.Base;
using System;

/// <summary>
///   ArgMax distance (L0) distance.
/// </summary>
/// 
[Serializable]
public struct ArgMax : IDistance<double[]>, ICloneable
{
    /// <summary>
    ///   Computes the distance <c>d(x,y)</c> between points
    ///   <paramref name="x"/> and <paramref name="y"/>.
    /// </summary>
    /// 
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>
    ///   A double-precision value representing the distance <c>d(x,y)</c>
    ///   between <paramref name="x"/> and <paramref name="y"/> according 
    ///   to the distance function implemented by this class.
    /// </returns>
    /// 
    public double Distance(double[] x, double[] y)
    {
        int xx = x.ArgMax();
        int yy = y.ArgMax();
        if (xx != yy)
            return 0;
        return 1;
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new ArgMax();
    }
}
