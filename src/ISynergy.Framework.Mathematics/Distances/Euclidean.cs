namespace ISynergy.Framework.Mathematics.Distances;

using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Distances.Base;
using System;

/// <summary>
///   Euclidean distance metric.
/// </summary>
/// 
/// <seealso cref="SquareEuclidean"/>
/// <seealso cref="WeightedSquareEuclidean"/>
/// <seealso cref="WeightedEuclidean"/>
/// 
[Serializable]
public struct Euclidean :
    IMetric<double>, ISimilarity<double>,
    IMetric<double[]>, ISimilarity<double[]>,
    IMetric<Tuple<double, double>>, ISimilarity<Tuple<double, double>>,
    IDistance<Sparse<double>>, ISimilarity<Sparse<double>>,
    ICloneable
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
    public double Distance(double x, double y)
    {
        return Math.Abs(x - y);
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
        double sum = 0.0;
        for (var i = 0; i < x.Length; i++)
        {
            double u = x[i] - y[i];
            sum += u * u;
        }
        return Math.Sqrt(sum);
    }
    /// <summary>
    ///   Gets the Euclidean distance between two points. Note: this function 
    ///   is dangerous as it is too easy to invert its arguments by mistake. 
    ///   Please consider using the Tuple&lt;double, double> overload instead.
    /// </summary>
    /// 
    /// <param name="vector1x">The first coordinate of first point in space.</param>
    /// <param name="vector1y">The second coordinate of first point in space.</param>
    /// <param name="vector2x">The first coordinate of second point in space.</param>
    /// <param name="vector2y">The second coordinate of second point in space.</param>
    /// 
    /// <returns>The Euclidean distance between x and y.</returns>
    /// 
    public double Distance(double vector1x, double vector1y, double vector2x, double vector2y)
    {
        double dx = vector1x - vector2x;
        double dy = vector1y - vector2y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    ///   Gets the Euclidean distance between two points.
    /// </summary>
    /// 
    /// <param name="x">The first point in space.</param>
    /// <param name="y">The second point in space.</param>
    /// 
    /// <returns>The Euclidean distance between x and y.</returns>
    /// 
    public double Distance(Tuple<double, double> x, Tuple<double, double> y)
    {
        double dx = x.Item1 - y.Item1;
        double dy = x.Item2 - y.Item2;
        return Math.Sqrt(dx * dx + dy * dy);
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
    public double Distance(Sparse<double> x, Sparse<double> y)
    {
        return Math.Sqrt(SquareEuclidean.Sparse(x, y));
    }

    /// <summary>
    ///   Gets a similarity measure between two points.
    /// </summary>
    /// 
    /// <param name="x">The first point to be compared.</param>
    /// <param name="y">The second point to be compared.</param>
    /// 
    /// <returns>A similarity measure between x and y.</returns>
    /// 
    public double Similarity(double x, double y)
    {
        return 1.0 / (1.0 + Math.Abs(x - y));
    }

    /// <summary>
    ///   Gets a similarity measure between two points.
    /// </summary>
    /// 
    /// <param name="x">The first point to be compared.</param>
    /// <param name="y">The second point to be compared.</param>
    /// 
    /// <returns>A similarity measure between x and y.</returns>
    /// 
    public double Similarity(double[] x, double[] y)
    {
        double sum = 0.0;

        for (var i = 0; i < x.Length; i++)
        {
            double u = x[i] - y[i];
            sum += u * u;
        }

        return 1.0 / (1.0 + Math.Sqrt(sum));
    }

    /// <summary>
    ///   Gets a similarity measure between two points.
    /// </summary>
    /// 
    /// <param name="x">The first point to be compared.</param>
    /// <param name="y">The second point to be compared.</param>
    /// 
    /// <returns>A similarity measure between x and y.</returns>
    /// 
    public double Similarity(Tuple<double, double> x, Tuple<double, double> y)
    {
        double dx = x.Item1 - y.Item1;
        double dy = x.Item2 - y.Item2;

        return 1.0 / (1.0 + Math.Sqrt(dx * dx + dy * dy));
    }

    /// <summary>
    ///   Gets a similarity measure between two points.
    /// </summary>
    /// 
    /// <param name="x">The first point to be compared.</param>
    /// <param name="y">The second point to be compared.</param>
    /// 
    /// <returns>A similarity measure between x and y.</returns>
    /// 
    public double Similarity(Sparse<double> x, Sparse<double> y)
    {
        return 1.0 / (1.0 + Distance(x, y));
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new Euclidean();
    }

}
