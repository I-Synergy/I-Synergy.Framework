namespace ISynergy.Framework.Mathematics.Distances;

using ISynergy.Framework.Mathematics.Distances.Base;
using System;

/// <summary>
///   Matching dissimilarity.
/// </summary>
/// 
[Serializable]
public struct Matching : IDistance<double[]>, IDistance<int[]>, ICloneable
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
    public double Distance(int[] x, int[] y)
    {
        int tf = 0;
        int ft = 0;

        for (var i = 0; i < x.Length; i++)
        {
            if (x[i] == 1 && y[i] == 0) tf++;
            if (x[i] == 0 && y[i] == 1) ft++;
        }

        return (tf + ft) / (double)(x.Length);
    }

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
        int tf = 0;
        int ft = 0;

        for (var i = 0; i < x.Length; i++)
        {
            if (x[i] == 1 && y[i] == 0) tf++;
            if (x[i] == 0 && y[i] == 1) ft++;
        }

        return (tf + ft) / (double)(x.Length);
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new Matching();
    }
}
