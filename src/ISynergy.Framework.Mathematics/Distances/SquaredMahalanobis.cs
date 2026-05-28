using ISynergy.Framework.Mathematics.Decompositions;
using ISynergy.Framework.Mathematics.Distances.Base;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.Distances;

/// <summary>
///   Squared Mahalanobis distance.
/// </summary>
/// 
[Serializable]
public class SquareMahalanobis : BaseMahalanobis
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public SquareMahalanobis() : base() { }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
    /// </summary>
    /// 
    /// <param name="chol">A Cholesky decomposition of the covariance matrix.</param>
    /// 
    public SquareMahalanobis(CholeskyDecomposition chol)
        : base(chol) { }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
    /// </summary>
    /// 
    /// <param name="svd">A Singular Value decomposition of the covariance matrix.</param>
    /// 
    public SquareMahalanobis(SingularValueDecomposition svd)
        : base(svd) { }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
    /// </summary>
    /// 
    /// <param name="precision">The precision matrix (the inverse of the covariance matrix).</param>
    /// 
    public SquareMahalanobis(double[,] precision)
        : base(precision) { }

    /// <summary>
    ///   Creates a new Square-Mahalanobis distance from a covariance matrix.
    /// </summary>
    /// 
    /// <param name="covariance">A covariance matrix.</param>
    /// 
    /// <returns>
    ///   A square Mahalanobis distance using the <see cref="SingularValueDecomposition"/>
    ///   of the given covariance matrix.
    /// </returns>
    /// 
    public static SquareMahalanobis FromCovarianceMatrix(double[,] covariance)
    {
        return new SquareMahalanobis(new CholeskyDecomposition(covariance));
    }

    /// <summary>
    ///   Creates a new Square-Mahalanobis distance from a precision matrix.
    /// </summary>
    /// 
    /// <param name="precision">A precision matrix.</param>
    /// 
    /// <returns>
    ///   A square Mahalanobis distance using the given precision matrix.
    /// </returns>
    /// 
    public static SquareMahalanobis FromPrecisionMatrix(double[,] precision)
    {
        return new SquareMahalanobis(precision);
    }
}
