using ISynergy.Framework.Mathematics.Distances.Base;
using ISynergy.Framework.Mathematics.Vectors;

namespace ISynergy.Framework.Mathematics.Distances;

/// <summary>
///   Weighted Square-Euclidean distance and similarity. Please note that this
///   distance is not a metric as it doesn't obey the triangle inequality.
/// </summary>
/// 
/// <seealso cref="Euclidean"/>
/// <seealso cref="WeightedEuclidean"/>
/// <seealso cref="WeightedSquareEuclidean"/>
///
[Serializable]
public struct WeightedSquareEuclidean : IDistance<double[]>, ISimilarity<double[]>, ICloneable
{
    private double[] weights;
    /// <summary>
    /// Gets or sets the weights for each dimension. Default is a vector of ones.
    /// </summary>
    /// 
    /// <value>The weights.</value>
    /// 
    public double[] Weights
    {
        get { return weights; }
        set { weights = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeightedSquareEuclidean"/> struct.
    /// </summary>
    /// <param name="dimensions">The number of dimensions (columns) in the dataset.</param>
    public WeightedSquareEuclidean(int dimensions)
    {
        this.weights = Vector.Ones(dimensions);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeightedSquareEuclidean"/> struct.
    /// </summary>
    /// <param name="weights">The weights.</param>
    public WeightedSquareEuclidean(double[] weights)
    {
        this.weights = weights;
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
            sum += u * u * weights[i];
        }
        return sum;
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
        return 1.0 / (1.0 + Distance(x, y));
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new WeightedSquareEuclidean((double[])weights.Clone());
    }
}
