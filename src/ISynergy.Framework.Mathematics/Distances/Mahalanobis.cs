using ISynergy.Framework.Mathematics.Decompositions;
using ISynergy.Framework.Mathematics.Distances.Base;

namespace ISynergy.Framework.Mathematics.Distances
{
    /// <summary>
    ///   Mahalanobis distance.
    /// </summary>
    /// 
    [Serializable]
    public class Mahalanobis : BaseMahalanobis
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Mahalanobis() : base() { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
        /// </summary>
        /// 
        /// <param name="chol">A Cholesky decomposition of the covariance matrix.</param>
        /// 
        public Mahalanobis(CholeskyDecomposition chol)
            : base(chol) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
        /// </summary>
        /// 
        /// <param name="svd">A Singular Value decomposition of the covariance matrix.</param>
        /// 
        public Mahalanobis(SingularValueDecomposition svd)
            : base(svd) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
        /// </summary>
        /// 
        /// <param name="precision">The precision matrix (the inverse of the covariance matrix).</param>
        /// 
        public Mahalanobis(double[,] precision)
            : base(precision) { }

        /// <summary>
        ///   Creates a new Mahalanobis distance from a covariance matrix.
        /// </summary>
        /// 
        /// <param name="covariance">A covariance matrix.</param>
        /// 
        /// <returns>
        ///   A Mahalanobis distance using the <see cref="SingularValueDecomposition"/>
        ///   of the given covariance matrix.
        /// </returns>
        /// 
        public static Mahalanobis FromCovarianceMatrix(double[,] covariance)
        {
            return new Mahalanobis(new CholeskyDecomposition(covariance));
        }

        /// <summary>
        ///   Creates a new Mahalanobis distance from a precision matrix.
        /// </summary>
        /// 
        /// <param name="precision">A precision matrix.</param>
        /// 
        /// <returns>
        ///   A Mahalanobis distance using the given precision matrix.
        /// </returns>
        /// 
        public static Mahalanobis FromPrecisionMatrix(double[,] precision)
        {
            return new Mahalanobis(precision);
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
        public override double Distance(double[] x, double[] y)
        {
            return Math.Sqrt(base.Distance(x, y));
        }
    }
}
