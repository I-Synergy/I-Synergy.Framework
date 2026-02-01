using ISynergy.Framework.Mathematics.Decompositions;
using ISynergy.Framework.Mathematics.Distances.Base;

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
