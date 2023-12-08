namespace ISynergy.Framework.Mathematics.Distances;

using ISynergy.Framework.Mathematics.Distances.Base;
using System;

/// <summary>
///   Pearson Correlation similarity.
/// </summary>
/// 
[Serializable]
public struct PearsonCorrelation : ISimilarity<double[]>, ICloneable
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
    public double Similarity(double[] x, double[] y)
    {
        double p = 0;
        double q = 0;
        double p2 = 0;
        double q2 = 0;
        double sum = 0;

        for (var i = 0; i < x.Length; i++)
        {
            p += x[i];
            q += y[i];
            p2 += x[i] * x[i];
            q2 += y[i] * y[i];
            sum += x[i] * y[i];
        }

        double n = x.Length;
        double num = sum - (p * q) / n;
        double den = Math.Sqrt((p2 - (p * p) / n) * (q2 - (q * q) / n));

        return (den == 0) ? 0 : num / den;
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new PearsonCorrelation();
    }
}
