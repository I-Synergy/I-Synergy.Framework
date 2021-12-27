using ISynergy.Framework.Mathematics.Decompositions;
using System;

namespace ISynergy.Framework.Mathematics.Distances.Base
{
    /// <summary>
    /// Base Mahalanobis class.
    /// </summary>
    public abstract class BaseMahalanobis : IMetric<double[]>, ICloneable
    {
        private readonly CholeskyDecomposition _chol;
        private readonly SingularValueDecomposition _svd;
        private readonly double[,] _precision;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        protected BaseMahalanobis()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
        /// </summary>
        /// 
        /// <param name="chol">A Cholesky decomposition of the covariance matrix.</param>
        /// 
        protected BaseMahalanobis(CholeskyDecomposition chol)
        {
            _chol = chol;
            _svd = null;
            _precision = null;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
        /// </summary>
        /// 
        /// <param name="svd">A Singular Value decomposition of the covariance matrix.</param>
        /// 
        protected BaseMahalanobis(SingularValueDecomposition svd)
        {
            _chol = null;
            _svd = svd;
            _precision = null;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
        /// </summary>
        /// 
        /// <param name="precision">The precision matrix (the inverse of the covariance matrix).</param>
        /// 
        protected BaseMahalanobis(double[,] precision)
        {
            _chol = null;
            _svd = null;
            _precision = precision;
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
        public virtual double Distance(double[] x, double[] y)
        {
            double[] d = new double[x.Length];

            for (var i = 0; i < x.Length; i++)
                d[i] = x[i] - y[i];

            double[] z;

            if (_svd is not null)
                z = _svd.Solve(d);
            else if (_chol is not null)
                z = _chol.Solve(d);
            else
                z = _precision.Dot(d);

            double sum = 0.0;
            for (var i = 0; i < d.Length; i++)
                sum += d[i] * z[i];

            return Math.Abs(sum);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
