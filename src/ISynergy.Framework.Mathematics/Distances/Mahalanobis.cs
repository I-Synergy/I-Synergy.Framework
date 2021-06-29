namespace ISynergy.Framework.Mathematics.Distances
{
    using ISynergy.Framework.Mathematics.Decompositions;
    using System;

    /// <summary>
    ///   Mahalanobis distance.
    /// </summary>
    /// 
    /// <example>
    ///   <code source="tests\ISynergy.Framework.Mathematics.Tests.Math\DistanceTest.cs" region="doc_mahalanobis_3" />
    /// </example>
    /// 
    [Serializable]
    public struct Mahalanobis : IMetric<double[]>, ICloneable
    {
        CholeskyDecomposition chol;
        SingularValueDecomposition svd;
        double[,] precision;

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
        /// </summary>
        /// 
        /// <param name="chol">A Cholesky decomposition of the covariance matrix.</param>
        /// 
        /// <example>
        ///   <code source="tests\ISynergy.Framework.Mathematics.Tests.Math\DistanceTest.cs" region="doc_mahalanobis_3" />
        /// </example>
        /// 
        public Mahalanobis(CholeskyDecomposition chol)
        {
            this.chol = chol;
            this.svd = null;
            this.precision = null;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
        /// </summary>
        /// 
        /// <param name="svd">A Singular Value decomposition of the covariance matrix.</param>
        /// 
        /// <example>
        ///   <code source="tests\ISynergy.Framework.Mathematics.Tests.Math\DistanceTest.cs" region="doc_mahalanobis_3" />
        /// </example>
        /// 
        public Mahalanobis(SingularValueDecomposition svd)
        {
            this.chol = null;
            this.svd = svd;
            this.precision = null;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
        /// </summary>
        /// 
        /// <param name="precision">The precision matrix (the inverse of the covariance matrix).</param>
        /// 
        /// <example>
        ///   <code source="tests\ISynergy.Framework.Mathematics.Tests.Math\DistanceTest.cs" region="doc_mahalanobis_3" />
        /// </example>
        /// 
        public Mahalanobis(double[,] precision)
        {
            this.chol = null;
            this.svd = null;
            this.precision = precision;
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
        /// <example>
        ///   <code source="tests\ISynergy.Framework.Mathematics.Tests.Math\DistanceTest.cs" region="doc_mahalanobis_3" />
        /// </example>
        /// 
        public double Distance(double[] x, double[] y)
        {
            double[] d = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
                d[i] = x[i] - y[i];

            double[] z;
            if (svd != null)
                z = svd.Solve(d);
            else if (chol != null)
                z = chol.Solve(d);
            else
                z = precision.Dot(d);

            double sum = 0.0;
            for (int i = 0; i < d.Length; i++)
                sum += d[i] * z[i];
            return Math.Sqrt(Math.Abs(sum));
        }

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
        /// <example>
        ///   <code source="tests\ISynergy.Framework.Mathematics.Tests.Math\DistanceTest.cs" region="doc_mahalanobis_3" />
        /// </example>
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
        /// <example>
        ///   <code source="tests\ISynergy.Framework.Mathematics.Tests.Math\DistanceTest.cs" region="doc_mahalanobis_3" />
        /// </example>
        /// 
        public static Mahalanobis FromPrecisionMatrix(double[,] precision)
        {
            return new Mahalanobis(precision);
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
