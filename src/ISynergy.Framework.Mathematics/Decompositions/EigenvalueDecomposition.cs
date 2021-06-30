using System;

namespace ISynergy.Framework.Mathematics.Decompositions
{
    /// <summary>
    ///     Determines the eigenvalues and eigenvectors of a real square matrix.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         In the mathematical discipline of linear algebra, eigendecomposition
    ///         or sometimes spectral decomposition is the factorization of a matrix
    ///         into a canonical form, whereby the matrix is represented in terms of
    ///         its eigenvalues and eigenvectors.
    ///     </para>
    ///     <para>
    ///         If <c>A</c> is symmetric, then <c>A = V * D * V'</c> and <c>A = V * V'</c>
    ///         where the eigenvalue matrix <c>D</c> is diagonal and the eigenvector matrix <c>V</c> is orthogonal.
    ///         If <c>A</c> is not symmetric, the eigenvalue matrix <c>D</c> is block diagonal
    ///         with the real eigenvalues in 1-by-1 blocks and any complex eigenvalues,
    ///         <c>lambda + i*mu</c>, in 2-by-2 blocks, <c>[lambda, mu; -mu, lambda]</c>.
    ///         The columns of <c>V</c> represent the eigenvectors in the sense that <c>A * V = V * D</c>.
    ///         The matrix V may be badly conditioned, or even singular, so the validity of the equation
    ///         <c>A = V * D * inverse(V)</c> depends upon the condition of <c>V</c>.
    ///     </para>
    /// </remarks>
    public sealed class EigenvalueDecomposition : ICloneable
    {
        private const double eps = 2 * Constants.DoubleEpsilon;
        private double[,] diagonalMatrix;
        private double[,] H; // storage of nonsymmetric Hessenberg form.
        private int n; // matrix dimension
        private double[] ort; // storage for nonsymmetric algorithm.

        private int? rank;
        private bool symmetric;

        /// <summary>
        ///     Construct an eigenvalue decomposition.
        /// </summary>
        /// <param name="value">
        ///     The matrix to be decomposed.
        /// </param>
        /// <param name="inPlace">
        ///     Pass <see langword="true" /> to perform the decomposition in place. The matrix
        ///     <paramref name="value" /> will be destroyed in the process, resulting in less
        ///     memory comsumption.
        /// </param>
        /// <param name="sort">
        ///     Pass <see langword="true" /> to sort the eigenvalues and eigenvectors at the end
        ///     of the decomposition.
        /// </param>
        public EigenvalueDecomposition(double[,] value, bool inPlace = false, bool sort = false)
            : this(value, value.IsSymmetric(), inPlace, sort)
        {
        }

        /// <summary>
        ///     Construct an eigenvalue decomposition.
        /// </summary>
        /// <param name="value">
        ///     The matrix to be decomposed.
        /// </param>
        /// <param name="assumeSymmetric">
        ///     Defines if the matrix should be assumed as being symmetric
        ///     regardless if it is or not. Default is <see langword="false" />.
        /// </param>
        /// <param name="inPlace">
        ///     Pass <see langword="true" /> to perform the decomposition in place. The matrix
        ///     <paramref name="value" /> will be destroyed in the process, resulting in less
        ///     memory comsumption.
        /// </param>
        /// <param name="sort">
        ///     Pass <see langword="true" /> to sort the eigenvalues and eigenvectors at the end
        ///     of the decomposition.
        /// </param>
        public EigenvalueDecomposition(double[,] value, bool assumeSymmetric,
            bool inPlace = false, bool sort = false)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Matrix cannot be null.");

            if (value.GetLength(0) != value.GetLength(1))
                throw new ArgumentException("Matrix is not a square matrix.", "value");

            n = value.GetLength(1);
            Eigenvectors = new double[n, n];
            RealEigenvalues = new double[n];
            ImaginaryEigenvalues = new double[n];
            symmetric = assumeSymmetric;

            if (symmetric)
            {
                Eigenvectors = inPlace ? value : (double[,])value.Clone();

                // Tridiagonalize.
                tred2();

                // Diagonalize.
                tql2();
            }
            else
            {
                H = inPlace ? value : (double[,])value.Clone();

                ort = new double[n];

                // Reduce to Hessenberg form.
                orthes();

                // Reduce Hessenberg to real Schur form.
                hqr2();
            }

            if (sort)
            {
                // Sort eigenvalues and vectors in descending order
                var idx = Vector.Range(n);
                Array.Sort(idx, (i, j) =>
                {
                    if (Math.Abs(RealEigenvalues[i]) == Math.Abs(RealEigenvalues[j]))
                        return -Math.Abs(ImaginaryEigenvalues[i]).CompareTo(Math.Abs(ImaginaryEigenvalues[j]));
                    return -Math.Abs(RealEigenvalues[i]).CompareTo(Math.Abs(RealEigenvalues[j]));
                });

                RealEigenvalues = RealEigenvalues.Get(idx);
                ImaginaryEigenvalues = ImaginaryEigenvalues.Get(idx);
                Eigenvectors = Eigenvectors.Get(null, idx);
            }
        }
        /// <summary>
        ///     Returns the effective numerical matrix rank.
        /// </summary>
        /// <value>Number of non-negligible eigen values.</value>
        public int Rank
        {
            get
            {
                if (rank.HasValue)
                    return rank.Value;

                var tol = n * RealEigenvalues[0] * eps;

                var r = 0;
                for (var i = 0; i < RealEigenvalues.Length; i++)
                    if (RealEigenvalues[i] > tol)
                        r++;

                return (int)(rank = r);
            }
        }
        /// <summary>Returns the real parts of the eigenvalues.</summary>
        public double[] RealEigenvalues { get; private set; }

        /// <summary>Returns the imaginary parts of the eigenvalues.</summary>
        public double[] ImaginaryEigenvalues { get; private set; }

        /// <summary>Returns the eigenvector matrix.</summary>
        public double[,] Eigenvectors { get; private set; }

        /// <summary>Returns the block diagonal eigenvalue matrix.</summary>
        public double[,] DiagonalMatrix
        {
            get
            {
                if (diagonalMatrix != null)
                    return diagonalMatrix;

                var x = new double[n, n];

                for (var i = 0; i < n; i++)
                {
                    for (var j = 0; j < n; j++)
                        x[i, j] = 0;

                    x[i, i] = RealEigenvalues[i];
                    if (ImaginaryEigenvalues[i] > 0)
                        x[i, i + 1] = ImaginaryEigenvalues[i];
                    else if (ImaginaryEigenvalues[i] < 0) x[i, i - 1] = ImaginaryEigenvalues[i];
                }

                return diagonalMatrix = x;
            }
        }

        /// <summary>
        ///     Reverses the decomposition, reconstructing the original matrix <c>X</c>.
        /// </summary>
        public double[,] Reverse()
        {
            return Eigenvectors.DotWithDiagonal(RealEigenvalues).Divide(Eigenvectors);
        }
        #region Private methods

        private void tred2()
        {
            // Symmetric Householder reduction to tridiagonal form.
            // This is derived from the Algol procedures tred2 by Bowdler, Martin, Reinsch, and Wilkinson, 
            // Handbook for Auto. Comp., Vol.ii-Linear Algebra, and the corresponding Fortran subroutine in EISPACK.
            for (var j = 0; j < n; j++)
                RealEigenvalues[j] = Eigenvectors[n - 1, j];

            // Householder reduction to tridiagonal form.
            for (var i = n - 1; i > 0; i--)
            {
                // Scale to avoid under/overflow.
                double scale = 0;
                double h = 0;
                for (var k = 0; k < i; k++)
                    scale = scale + Math.Abs(RealEigenvalues[k]);

                if (scale == 0)
                {
                    ImaginaryEigenvalues[i] = RealEigenvalues[i - 1];
                    for (var j = 0; j < i; j++)
                    {
                        RealEigenvalues[j] = Eigenvectors[i - 1, j];
                        Eigenvectors[i, j] = 0;
                        Eigenvectors[j, i] = 0;
                    }
                }
                else
                {
                    // Generate Householder vector.
                    for (var k = 0; k < i; k++)
                    {
                        RealEigenvalues[k] /= scale;
                        h += RealEigenvalues[k] * RealEigenvalues[k];
                    }

                    var f = RealEigenvalues[i - 1];
                    var g = Math.Sqrt(h);
                    if (f > 0) g = -g;

                    ImaginaryEigenvalues[i] = scale * g;
                    h = h - f * g;
                    RealEigenvalues[i - 1] = f - g;
                    for (var j = 0; j < i; j++)
                        ImaginaryEigenvalues[j] = 0;

                    // Apply similarity transformation to remaining columns.
                    for (var j = 0; j < i; j++)
                    {
                        f = RealEigenvalues[j];
                        Eigenvectors[j, i] = f;
                        g = ImaginaryEigenvalues[j] + Eigenvectors[j, j] * f;
                        for (var k = j + 1; k <= i - 1; k++)
                        {
                            g += Eigenvectors[k, j] * RealEigenvalues[k];
                            ImaginaryEigenvalues[k] += Eigenvectors[k, j] * f;
                        }

                        ImaginaryEigenvalues[j] = g;
                    }

                    f = 0;
                    for (var j = 0; j < i; j++)
                    {
                        ImaginaryEigenvalues[j] /= h;
                        f += ImaginaryEigenvalues[j] * RealEigenvalues[j];
                    }

                    var hh = f / (h + h);
                    for (var j = 0; j < i; j++)
                        ImaginaryEigenvalues[j] -= hh * RealEigenvalues[j];

                    for (var j = 0; j < i; j++)
                    {
                        f = RealEigenvalues[j];
                        g = ImaginaryEigenvalues[j];
                        for (var k = j; k <= i - 1; k++)
                            Eigenvectors[k, j] -= f * ImaginaryEigenvalues[k] + g * RealEigenvalues[k];

                        RealEigenvalues[j] = Eigenvectors[i - 1, j];
                        Eigenvectors[i, j] = 0;
                    }
                }

                RealEigenvalues[i] = h;
            }

            // Accumulate transformations.
            for (var i = 0; i < n - 1; i++)
            {
                Eigenvectors[n - 1, i] = Eigenvectors[i, i];
                Eigenvectors[i, i] = 1;
                var h = RealEigenvalues[i + 1];
                if (h != 0)
                {
                    for (var k = 0; k <= i; k++)
                        RealEigenvalues[k] = Eigenvectors[k, i + 1] / h;

                    for (var j = 0; j <= i; j++)
                    {
                        double g = 0;
                        for (var k = 0; k <= i; k++)
                            g += Eigenvectors[k, i + 1] * Eigenvectors[k, j];
                        for (var k = 0; k <= i; k++)
                            Eigenvectors[k, j] -= g * RealEigenvalues[k];
                    }
                }

                for (var k = 0; k <= i; k++)
                    Eigenvectors[k, i + 1] = 0;
            }

            for (var j = 0; j < n; j++)
            {
                RealEigenvalues[j] = Eigenvectors[n - 1, j];
                Eigenvectors[n - 1, j] = 0;
            }

            Eigenvectors[n - 1, n - 1] = 1;
            ImaginaryEigenvalues[0] = 0;
        }

        private void tql2()
        {
            // Symmetric tridiagonal QL algorithm.
            // This is derived from the Algol procedures tql2, by Bowdler, Martin, Reinsch, and Wilkinson, 
            // Handbook for Auto. Comp., Vol.ii-Linear Algebra, and the corresponding Fortran subroutine in EISPACK.
            for (var i = 1; i < n; i++)
                ImaginaryEigenvalues[i - 1] = ImaginaryEigenvalues[i];

            ImaginaryEigenvalues[n - 1] = 0;

            double f = 0;
            double tst1 = 0;
            var eps = 2 * Constants.DoubleEpsilon;

            for (var l = 0; l < n; l++)
            {
                // Find small subdiagonal element.
                tst1 = Math.Max(tst1, Math.Abs(RealEigenvalues[l]) + Math.Abs(ImaginaryEigenvalues[l]));
                var m = l;
                while (m < n)
                {
                    if (Math.Abs(ImaginaryEigenvalues[m]) <= eps * tst1)
                        break;
                    m++;
                }

                // If m == l, d[l] is an eigenvalue, otherwise, iterate.
                if (m > l)
                {
                    var iter = 0;
                    do
                    {
                        iter = iter + 1; // (Could check iteration count here.)

                        // Compute implicit shift
                        var g = RealEigenvalues[l];
                        var p = (RealEigenvalues[l + 1] - g) / (2 * ImaginaryEigenvalues[l]);
                        var r = Tools.Hypotenuse(p, 1);
                        if (p < 0) r = -r;

                        RealEigenvalues[l] = ImaginaryEigenvalues[l] / (p + r);
                        RealEigenvalues[l + 1] = ImaginaryEigenvalues[l] * (p + r);
                        var dl1 = RealEigenvalues[l + 1];
                        var h = g - RealEigenvalues[l];
                        for (var i = l + 2; i < n; i++) RealEigenvalues[i] -= h;

                        f = f + h;

                        // Implicit QL transformation.
                        p = RealEigenvalues[m];
                        double c = 1;
                        var c2 = c;
                        var c3 = c;
                        var el1 = ImaginaryEigenvalues[l + 1];
                        double s = 0;
                        double s2 = 0;
                        for (var i = m - 1; i >= l; i--)
                        {
                            c3 = c2;
                            c2 = c;
                            s2 = s;
                            g = c * ImaginaryEigenvalues[i];
                            h = c * p;
                            r = Tools.Hypotenuse(p, ImaginaryEigenvalues[i]);
                            ImaginaryEigenvalues[i + 1] = s * r;
                            s = ImaginaryEigenvalues[i] / r;
                            c = p / r;
                            p = c * RealEigenvalues[i] - s * g;
                            RealEigenvalues[i + 1] = h + s * (c * g + s * RealEigenvalues[i]);

                            // Accumulate transformation.
                            for (var k = 0; k < n; k++)
                            {
                                h = Eigenvectors[k, i + 1];
                                Eigenvectors[k, i + 1] = s * Eigenvectors[k, i] + c * h;
                                Eigenvectors[k, i] = c * Eigenvectors[k, i] - s * h;
                            }
                        }

                        p = -s * s2 * c3 * el1 * ImaginaryEigenvalues[l] / dl1;
                        ImaginaryEigenvalues[l] = s * p;
                        RealEigenvalues[l] = c * p;

                        // Check for convergence.
                    } while (Math.Abs(ImaginaryEigenvalues[l]) > eps * tst1);
                }

                RealEigenvalues[l] = RealEigenvalues[l] + f;
                ImaginaryEigenvalues[l] = 0;
            }

            // Sort eigenvalues and corresponding vectors.
            for (var i = 0; i < n - 1; i++)
            {
                var k = i;
                var p = RealEigenvalues[i];
                for (var j = i + 1; j < n; j++)
                    if (RealEigenvalues[j] < p)
                    {
                        k = j;
                        p = RealEigenvalues[j];
                    }

                if (k != i)
                {
                    RealEigenvalues[k] = RealEigenvalues[i];
                    RealEigenvalues[i] = p;
                    for (var j = 0; j < n; j++)
                    {
                        p = Eigenvectors[j, i];
                        Eigenvectors[j, i] = Eigenvectors[j, k];
                        Eigenvectors[j, k] = p;
                    }
                }
            }
        }

        private void orthes()
        {
            // Nonsymmetric reduction to Hessenberg form.
            // This is derived from the Algol procedures orthes and ortran, by Martin and Wilkinson, 
            // Handbook for Auto. Comp., Vol.ii-Linear Algebra, and the corresponding Fortran subroutines in EISPACK.
            var low = 0;
            var high = n - 1;

            for (var m = low + 1; m <= high - 1; m++)
            {
                // Scale column.

                double scale = 0;
                for (var i = m; i <= high; i++)
                    scale = scale + Math.Abs(H[i, m - 1]);

                if (scale != 0)
                {
                    // Compute Householder transformation.
                    double h = 0;
                    for (var i = high; i >= m; i--)
                    {
                        ort[i] = H[i, m - 1] / scale;
                        h += ort[i] * ort[i];
                    }

                    var g = Math.Sqrt(h);
                    if (ort[m] > 0) g = -g;

                    h = h - ort[m] * g;
                    ort[m] = ort[m] - g;

                    // Apply Householder similarity transformation
                    // H = (I - u * u' / h) * H * (I - u * u') / h)
                    for (var j = m; j < n; j++)
                    {
                        double f = 0;
                        for (var i = high; i >= m; i--)
                            f += ort[i] * H[i, j];

                        f = f / h;
                        for (var i = m; i <= high; i++)
                            H[i, j] -= f * ort[i];
                    }

                    for (var i = 0; i <= high; i++)
                    {
                        double f = 0;
                        for (var j = high; j >= m; j--)
                            f += ort[j] * H[i, j];

                        f = f / h;
                        for (var j = m; j <= high; j++)
                            H[i, j] -= f * ort[j];
                    }

                    ort[m] = scale * ort[m];
                    H[m, m - 1] = scale * g;
                }
            }

            // Accumulate transformations (Algol's ortran).
            for (var i = 0; i < n; i++)
                for (var j = 0; j < n; j++)
                    Eigenvectors[i, j] = i == j ? 1 : 0;

            for (var m = high - 1; m >= low + 1; m--)
                if (H[m, m - 1] != 0)
                {
                    for (var i = m + 1; i <= high; i++)
                        ort[i] = H[i, m - 1];

                    for (var j = m; j <= high; j++)
                    {
                        double g = 0;
                        for (var i = m; i <= high; i++)
                            g += ort[i] * Eigenvectors[i, j];

                        // Double division avoids possible underflow.
                        g = g / ort[m] / H[m, m - 1];
                        for (var i = m; i <= high; i++)
                            Eigenvectors[i, j] += g * ort[i];
                    }
                }
        }

        private static void cdiv(double xr, double xi, double yr, double yi,
            out double cdivr, out double cdivi)
        {
            // Complex scalar division.
            double r;
            double d;
            if (Math.Abs(yr) > Math.Abs(yi))
            {
                r = yi / yr;
                d = yr + r * yi;
                cdivr = (xr + r * xi) / d;
                cdivi = (xi - r * xr) / d;
            }
            else
            {
                r = yr / yi;
                d = yi + r * yr;
                cdivr = (r * xr + xi) / d;
                cdivi = (r * xi - xr) / d;
            }
        }

        private void hqr2()
        {
            // Nonsymmetric reduction from Hessenberg to real Schur form.   
            // This is derived from the Algol procedure hqr2, by Martin and Wilkinson, Handbook for Auto. Comp.,
            // Vol.ii-Linear Algebra, and the corresponding  Fortran subroutine in EISPACK.
            var nn = this.n;
            var n = nn - 1;
            var low = 0;
            var high = nn - 1;
            var eps = 2 * Constants.DoubleEpsilon;
            double exshift = 0;
            double p = 0;
            double q = 0;
            double r = 0;
            double s = 0;
            double z = 0;
            double t;
            double w;
            double x;
            double y;

            // Store roots isolated by balanc and compute matrix norm
            double norm = 0;
            for (var i = 0; i < nn; i++)
            {
                if ((i < low) | (i > high))
                {
                    RealEigenvalues[i] = H[i, i];
                    ImaginaryEigenvalues[i] = 0;
                }

                for (var j = Math.Max(i - 1, 0); j < nn; j++)
                    norm = norm + Math.Abs(H[i, j]);
            }

            // Outer loop over eigenvalue index
            var iter = 0;
            while (n >= low)
            {
                // Look for single small sub-diagonal element
                var l = n;
                while (l > low)
                {
                    s = Math.Abs(H[l - 1, l - 1]) + Math.Abs(H[l, l]);

                    if (s == 0)
                        s = norm;

                    if (double.IsNaN(s))
                        break;

                    if (Math.Abs(H[l, l - 1]) < eps * s)
                        break;

                    l--;
                }

                // Check for convergence
                if (l == n)
                {
                    // One root found
                    H[n, n] = H[n, n] + exshift;
                    RealEigenvalues[n] = H[n, n];
                    ImaginaryEigenvalues[n] = 0;
                    n--;
                    iter = 0;
                }
                else if (l == n - 1)
                {
                    // Two roots found
                    w = H[n, n - 1] * H[n - 1, n];
                    p = (H[n - 1, n - 1] - H[n, n]) / 2;
                    q = p * p + w;
                    z = Math.Sqrt(Math.Abs(q));
                    H[n, n] = H[n, n] + exshift;
                    H[n - 1, n - 1] = H[n - 1, n - 1] + exshift;
                    x = H[n, n];

                    if (q >= 0)
                    {
                        // Real pair
                        z = p >= 0 ? p + z : p - z;
                        RealEigenvalues[n - 1] = x + z;
                        RealEigenvalues[n] = RealEigenvalues[n - 1];
                        if (z != 0)
                            RealEigenvalues[n] = x - w / z;
                        ImaginaryEigenvalues[n - 1] = 0;
                        ImaginaryEigenvalues[n] = 0;
                        x = H[n, n - 1];
                        s = Math.Abs(x) + Math.Abs(z);
                        p = x / s;
                        q = z / s;
                        r = Math.Sqrt(p * p + q * q);
                        p = p / r;
                        q = q / r;

                        // Row modification
                        for (var j = n - 1; j < nn; j++)
                        {
                            z = H[n - 1, j];
                            H[n - 1, j] = q * z + p * H[n, j];
                            H[n, j] = q * H[n, j] - p * z;
                        }

                        // Column modification
                        for (var i = 0; i <= n; i++)
                        {
                            z = H[i, n - 1];
                            H[i, n - 1] = q * z + p * H[i, n];
                            H[i, n] = q * H[i, n] - p * z;
                        }

                        // Accumulate transformations
                        for (var i = low; i <= high; i++)
                        {
                            z = Eigenvectors[i, n - 1];
                            Eigenvectors[i, n - 1] = q * z + p * Eigenvectors[i, n];
                            Eigenvectors[i, n] = q * Eigenvectors[i, n] - p * z;
                        }
                    }
                    else
                    {
                        // Complex pair
                        RealEigenvalues[n - 1] = x + p;
                        RealEigenvalues[n] = x + p;
                        ImaginaryEigenvalues[n - 1] = z;
                        ImaginaryEigenvalues[n] = -z;
                    }

                    n = n - 2;
                    iter = 0;
                }
                else
                {
                    // No convergence yet     

                    // Form shift
                    x = H[n, n];
                    y = 0;
                    w = 0;
                    if (l < n)
                    {
                        y = H[n - 1, n - 1];
                        w = H[n, n - 1] * H[n - 1, n];
                    }

                    // Wilkinson's original ad hoc shift
                    if (iter == 10)
                    {
                        exshift += x;
                        for (var i = low; i <= n; i++)
                            H[i, i] -= x;

                        s = Math.Abs(H[n, n - 1]) + Math.Abs(H[n - 1, n - 2]);
                        x = y = 0.75 * s;
                        w = -0.4375 * s * s;
                    }

                    // MATLAB's new ad hoc shift
                    if (iter == 30)
                    {
                        s = (y - x) / 2;
                        s = s * s + w;
                        if (s > 0)
                        {
                            s = Math.Sqrt(s);
                            if (y < x) s = -s;
                            s = x - w / ((y - x) / 2 + s);
                            for (var i = low; i <= n; i++)
                                H[i, i] -= s;
                            exshift += s;
                            x = y = w = 0.964;
                        }
                    }

                    iter = iter + 1;

                    // Look for two consecutive small sub-diagonal elements
                    var m = n - 2;
                    while (m >= l)
                    {
                        z = H[m, m];
                        r = x - z;
                        s = y - z;
                        p = (r * s - w) / H[m + 1, m] + H[m, m + 1];
                        q = H[m + 1, m + 1] - z - r - s;
                        r = H[m + 2, m + 1];
                        s = Math.Abs(p) + Math.Abs(q) + Math.Abs(r);
                        p = p / s;
                        q = q / s;
                        r = r / s;
                        if (m == l)
                            break;
                        if (Math.Abs(H[m, m - 1]) * (Math.Abs(q) + Math.Abs(r)) < eps * (Math.Abs(p) *
                            (Math.Abs(H[m - 1, m - 1]) + Math.Abs(z) + Math.Abs(H[m + 1, m + 1]))))
                            break;
                        m--;
                    }

                    for (var i = m + 2; i <= n; i++)
                    {
                        H[i, i - 2] = 0;
                        if (i > m + 2)
                            H[i, i - 3] = 0;
                    }

                    // Double QR step involving rows l:n and columns m:n
                    for (var k = m; k <= n - 1; k++)
                    {
                        var notlast = k != n - 1;
                        if (k != m)
                        {
                            p = H[k, k - 1];
                            q = H[k + 1, k - 1];
                            r = notlast ? H[k + 2, k - 1] : 0;
                            x = Math.Abs(p) + Math.Abs(q) + Math.Abs(r);
                            if (x != 0)
                            {
                                p = p / x;
                                q = q / x;
                                r = r / x;
                            }
                        }

                        if (x == 0) break;

                        s = Math.Sqrt(p * p + q * q + r * r);
                        if (p < 0) s = -s;

                        if (s != 0)
                        {
                            if (k != m)
                                H[k, k - 1] = -s * x;
                            else if (l != m)
                                H[k, k - 1] = -H[k, k - 1];

                            p = p + s;
                            x = p / s;
                            y = q / s;
                            z = r / s;
                            q = q / p;
                            r = r / p;

                            // Row modification
                            for (var j = k; j < nn; j++)
                            {
                                p = H[k, j] + q * H[k + 1, j];
                                if (notlast)
                                {
                                    p = p + r * H[k + 2, j];
                                    H[k + 2, j] = H[k + 2, j] - p * z;
                                }

                                H[k, j] = H[k, j] - p * x;
                                H[k + 1, j] = H[k + 1, j] - p * y;
                            }

                            // Column modification
                            for (var i = 0; i <= Math.Min(n, k + 3); i++)
                            {
                                p = x * H[i, k] + y * H[i, k + 1];
                                if (notlast)
                                {
                                    p = p + z * H[i, k + 2];
                                    H[i, k + 2] = H[i, k + 2] - p * r;
                                }

                                H[i, k] = H[i, k] - p;
                                H[i, k + 1] = H[i, k + 1] - p * q;
                            }

                            // Accumulate transformations
                            for (var i = low; i <= high; i++)
                            {
                                p = x * Eigenvectors[i, k] + y * Eigenvectors[i, k + 1];
                                if (notlast)
                                {
                                    p = p + z * Eigenvectors[i, k + 2];
                                    Eigenvectors[i, k + 2] = Eigenvectors[i, k + 2] - p * r;
                                }

                                Eigenvectors[i, k] = Eigenvectors[i, k] - p;
                                Eigenvectors[i, k + 1] = Eigenvectors[i, k + 1] - p * q;
                            }
                        }
                    }
                }
            }

            // Backsubstitute to find vectors of upper triangular form
            if (norm == 0) return;

            for (n = nn - 1; n >= 0; n--)
            {
                p = RealEigenvalues[n];
                q = ImaginaryEigenvalues[n];

                // Real vector
                if (q == 0)
                {
                    var l = n;
                    H[n, n] = 1;
                    for (var i = n - 1; i >= 0; i--)
                    {
                        w = H[i, i] - p;
                        r = 0;
                        for (var j = l; j <= n; j++)
                            r = r + H[i, j] * H[j, n];

                        if (ImaginaryEigenvalues[i] < 0)
                        {
                            z = w;
                            s = r;
                        }
                        else
                        {
                            l = i;
                            if (ImaginaryEigenvalues[i] == 0)
                            {
                                H[i, n] = w != 0 ? -r / w : -r / (eps * norm);
                            }
                            else
                            {
                                // Solve real equations
                                x = H[i, i + 1];
                                y = H[i + 1, i];
                                q = (RealEigenvalues[i] - p) * (RealEigenvalues[i] - p) +
                                    ImaginaryEigenvalues[i] * ImaginaryEigenvalues[i];
                                t = (x * s - z * r) / q;
                                H[i, n] = t;
                                H[i + 1, n] = Math.Abs(x) > Math.Abs(z) ? (-r - w * t) / x : (-s - y * t) / z;
                            }

                            // Overflow control
                            t = Math.Abs(H[i, n]);
                            if (eps * t * t > 1)
                                for (var j = i; j <= n; j++)
                                    H[j, n] = H[j, n] / t;
                        }
                    }
                }
                else if (q < 0)
                {
                    // Complex vector
                    var l = n - 1;

                    // Last vector component imaginary so matrix is triangular
                    if (Math.Abs(H[n, n - 1]) > Math.Abs(H[n - 1, n]))
                    {
                        H[n - 1, n - 1] = q / H[n, n - 1];
                        H[n - 1, n] = -(H[n, n] - p) / H[n, n - 1];
                    }
                    else
                    {
                        cdiv(0, -H[n - 1, n], H[n - 1, n - 1] - p, q, out H[n - 1, n - 1], out H[n - 1, n]);
                    }

                    H[n, n - 1] = 0;
                    H[n, n] = 1;
                    for (var i = n - 2; i >= 0; i--)
                    {
                        double ra, sa, vr, vi;
                        ra = 0;
                        sa = 0;
                        for (var j = l; j <= n; j++)
                        {
                            ra = ra + H[i, j] * H[j, n - 1];
                            sa = sa + H[i, j] * H[j, n];
                        }

                        w = H[i, i] - p;

                        if (ImaginaryEigenvalues[i] < 0)
                        {
                            z = w;
                            r = ra;
                            s = sa;
                        }
                        else
                        {
                            l = i;
                            if (ImaginaryEigenvalues[i] == 0)
                            {
                                cdiv(-ra, -sa, w, q, out H[i, n - 1], out H[i, n]);
                            }
                            else
                            {
                                // Solve complex equations
                                x = H[i, i + 1];
                                y = H[i + 1, i];
                                vr = (RealEigenvalues[i] - p) * (RealEigenvalues[i] - p) +
                                    ImaginaryEigenvalues[i] * ImaginaryEigenvalues[i] - q * q;
                                vi = (RealEigenvalues[i] - p) * 2 * q;
                                if ((vr == 0) & (vi == 0))
                                    vr = eps * norm * (Math.Abs(w) + Math.Abs(q) + Math.Abs(x) + Math.Abs(y) +
                                                       Math.Abs(z));
                                cdiv(x * r - z * ra + q * sa, x * s - z * sa - q * ra, vr, vi, out H[i, n - 1],
                                    out H[i, n]);
                                if (Math.Abs(x) > Math.Abs(z) + Math.Abs(q))
                                {
                                    H[i + 1, n - 1] = (-ra - w * H[i, n - 1] + q * H[i, n]) / x;
                                    H[i + 1, n] = (-sa - w * H[i, n] - q * H[i, n - 1]) / x;
                                }
                                else
                                {
                                    cdiv(-r - y * H[i, n - 1], -s - y * H[i, n], z, q, out H[i + 1, n - 1],
                                        out H[i + 1, n]);
                                }
                            }

                            // Overflow control
                            t = Math.Max(Math.Abs(H[i, n - 1]), Math.Abs(H[i, n]));
                            if (eps * t * t > 1)
                                for (var j = i; j <= n; j++)
                                {
                                    H[j, n - 1] = H[j, n - 1] / t;
                                    H[j, n] = H[j, n] / t;
                                }
                        }
                    }
                }
            }

            // Vectors of isolated roots
            for (var i = 0; i < nn; i++)
                if ((i < low) | (i > high))
                    for (var j = i; j < nn; j++)
                        Eigenvectors[i, j] = H[i, j];

            // Back transformation to get eigenvectors of original matrix
            for (var j = nn - 1; j >= low; j--)
                for (var i = low; i <= high; i++)
                {
                    z = 0;
                    for (var k = low; k <= Math.Min(j, high); k++)
                        z = z + Eigenvectors[i, k] * H[k, j];
                    Eigenvectors[i, j] = z;
                }
        }

        #endregion
        #region ICloneable Members

        private EigenvalueDecomposition()
        {
        }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var clone = new EigenvalueDecomposition();
            clone.RealEigenvalues = (double[])RealEigenvalues.Clone();
            clone.ImaginaryEigenvalues = (double[])ImaginaryEigenvalues.Clone();
            clone.H = (double[,])H.Clone();
            clone.n = n;
            clone.ort = ort;
            clone.symmetric = symmetric;
            clone.Eigenvectors = (double[,])Eigenvectors.Clone();
            return clone;
        }

        #endregion
    }
}