namespace ISynergy.Framework.Mathematics.Distances;

using ISynergy.Framework.Mathematics.Distances.Base;
using System;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


/// <summary>
///   Kulczynski dissimilarity.
/// </summary>
/// 
[Serializable]
public struct Kulczynski : IDistance<double[]>, IDistance<int[]>, ICloneable
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
        int tt = 0;

        for (var i = 0; i < x.Length; i++)
        {
            if (x[i] != 0 && y[i] == 0) tf++;
            if (x[i] == 0 && y[i] != 0) ft++;
            if (x[i] != 0 && y[i] != 0) tt++;
        }

        double num = tf + ft - tt + x.Length;
        double den = ft + tf + x.Length;
        return num / den;
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
        // TODO: Rewrite the integer dissimilarities (Yule, Russel-Rao,...)
        // using generics
        int tf = 0;
        int ft = 0;
        int tt = 0;

        for (var i = 0; i < x.Length; i++)
        {
            if (x[i] != 0 && y[i] == 0) tf++;
            if (x[i] == 0 && y[i] != 0) ft++;
            if (x[i] != 0 && y[i] != 0) tt++;
        }

        double num = tf + ft - tt + x.Length;
        double den = ft + tf + x.Length;
        return num / den;
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new Kulczynski();
    }
}
