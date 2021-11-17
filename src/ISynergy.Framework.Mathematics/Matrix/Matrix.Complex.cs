﻿namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    ///     Static class ComplexExtensions. Defines a set of extension methods
    ///     that operates mainly on multidimensional arrays and vectors of
    ///     AForge.NET's <seealso cref="Complex" /> data type.
    /// </summary>
    public static class ComplexMatrix
    {
        /// <summary>
        ///     Computes the absolute value of an array of complex numbers.
        /// </summary>
        public static Complex[] Abs(this Complex[] x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            var r = new Complex[x.Length];
            for (var i = 0; i < x.Length; i++)
                r[i] = new Complex(x[i].Magnitude, 0);
            return r;
        }

        /// <summary>
        ///     Computes the sum of an array of complex numbers.
        /// </summary>
        public static Complex Sum(this Complex[] x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            var r = Complex.Zero;
            for (var i = 0; i < x.Length; i++)
                r += x[i];
            return r;
        }

        /// <summary>
        ///     Elementwise multiplication of two complex vectors.
        /// </summary>
        public static Complex[] Multiply(this Complex[] a, Complex[] b)
        {
            if (a == null)
                throw new ArgumentNullException("a");
            if (b == null)
                throw new ArgumentNullException("b");

            var r = new Complex[a.Length];
            for (var i = 0; i < a.Length; i++) r[i] = Complex.Multiply(a[i], b[i]);
            return r;
        }

        /// <summary>
        ///     Gets the magnitude of every complex number in an array.
        /// </summary>
        public static double[] Magnitude(this Complex[] c)
        {
            return c.Apply((x, i) => x.Magnitude);
        }

        /// <summary>
        ///     Gets the magnitude of every complex number in a matrix.
        /// </summary>
        public static double[,] Magnitude(this Complex[,] c)
        {
            return c.Apply((x, i, j) => x.Magnitude);
        }

        /// <summary>
        ///     Gets the magnitude of every complex number in a matrix.
        /// </summary>
        public static double[][] Magnitude(this Complex[][] c)
        {
            return c.Apply((x, i, j) => x.Magnitude);
        }

        /// <summary>
        ///     Gets the phase of every complex number in an array.
        /// </summary>
        public static double[] Phase(this Complex[] c)
        {
            return c.Apply((x, i) => x.Phase);
        }

        /// <summary>
        ///     Gets the phase of every complex number in a matrix.
        /// </summary>
        public static double[,] Phase(this Complex[,] c)
        {
            return c.Apply((x, i, j) => x.Phase);
        }

        /// <summary>
        ///     Gets the phase of every complex number in a matrix.
        /// </summary>
        public static double[][] Phase(this Complex[][] c)
        {
            return c.Apply((x, i, j) => x.Phase);
        }

        /// <summary>
        ///     Gets the conjugate of every complex number in an array.
        /// </summary>
        public static Complex[] Conjugate(this Complex[] c)
        {
            return c.Apply((x, i) => Complex.Conjugate(x));
        }

        /// <summary>
        ///     Gets the conjugate of every complex number in a matrix.
        /// </summary>
        public static Complex[,] Conjugate(this Complex[,] c)
        {
            return c.Apply((x, i, j) => Complex.Conjugate(x));
        }

        /// <summary>
        ///     Gets the conjugate of every complex number in a matrix.
        /// </summary>
        public static Complex[][] Conjugate(this Complex[][] c)
        {
            return c.Apply((x, i, j) => Complex.Conjugate(x));
        }

        /// <summary>
        ///     Returns the real vector part of the complex vector c.
        /// </summary>
        /// <param name="c">A vector of complex numbers.</param>
        /// <returns>A vector of scalars with the real part of the complex numbers.</returns>
        public static double[] Re(this Complex[] c)
        {
            return c.Apply((x, i) => x.Real);
        }

        /// <summary>
        ///     Returns the real matrix part of the complex matrix c.
        /// </summary>
        /// <param name="c">A matrix of complex numbers.</param>
        /// <returns>A matrix of scalars with the real part of the complex numbers.</returns>
        public static double[,] Re(this Complex[,] c)
        {
            return c.Apply((x, i, j) => x.Real);
        }

        /// <summary>
        ///     Returns the real matrix part of the complex matrix c.
        /// </summary>
        /// <param name="c">A matrix of complex numbers.</param>
        /// <returns>A matrix of scalars with the real part of the complex numbers.</returns>
        public static double[][] Re(this Complex[][] c)
        {
            return c.Apply((x, i, j) => x.Real);
        }

        /// <summary>
        ///     Returns the imaginary vector part of the complex vector c.
        /// </summary>
        /// <param name="c">A vector of complex numbers.</param>
        /// <returns>A vector of scalars with the imaginary part of the complex numbers.</returns>
        // TODO: Rename to Imaginary
        public static double[] Im(this Complex[] c)
        {
            return c.Apply((x, i) => x.Imaginary);
        }

        /// <summary>
        ///     Returns the imaginary matrix part of the complex matrix c.
        /// </summary>
        /// <param name="c">A matrix of complex numbers.</param>
        /// <returns>A matrix of scalars with the imaginary part of the complex numbers.</returns>
        public static double[,] Im(this Complex[,] c)
        {
            return c.Apply((x, i, j) => x.Imaginary);
        }

        /// <summary>
        ///     Returns the imaginary matrix part of the complex matrix c.
        /// </summary>
        /// <param name="c">A matrix of complex numbers.</param>
        /// <returns>A matrix of scalars with the imaginary part of the complex numbers.</returns>
        public static double[][] Im(this Complex[][] c)
        {
            return c.Apply((x, i, j) => x.Imaginary);
        }

        /// <summary>
        ///     Converts a complex number to a matrix of scalar values
        ///     in which the first column contains the real values and
        ///     the second column contains the imaginary values.
        /// </summary>
        /// <param name="c">An array of complex numbers.</param>
        public static double[,] ToArray(this Complex[] c)
        {
            if (c == null)
                throw new ArgumentNullException("c");

            var arr = new double[c.Length, 2];
            for (var i = 0; i < c.GetLength(0); i++)
            {
                arr[i, 0] = c[i].Real;
                arr[i, 1] = c[i].Imaginary;
            }

            return arr;
        }

        /// <summary>
        ///     Combines a vector of real and a vector of
        ///     imaginary numbers to form complex numbers.
        /// </summary>
        /// <param name="real">The real part of the complex numbers.</param>
        /// <param name="imag">The imaginary part of the complex numbers</param>
        /// <returns>
        ///     A vector of complex number with the given
        ///     real numbers as their real components and
        ///     imaginary numbers as their imaginary parts.
        /// </returns>
        public static Complex[] ToComplex(this double[] real, double[] imag)
        {
            if (real == null)
                throw new ArgumentNullException("real");

            if (imag == null)
                throw new ArgumentNullException("imag");

            if (real.Length != imag.Length)
                throw new DimensionMismatchException("imag");

            var arr = new Complex[real.Length];
            for (var i = 0; i < arr.Length; i++)
                arr[i] = new Complex(real[i], imag[i]);

            return arr;
        }

        /// <summary>
        ///     Gets the range of the magnitude values in a complex number vector.
        /// </summary>
        /// <param name="array">A complex number vector.</param>
        /// <returns>The range of magnitude values in the complex vector.</returns>
        public static NumericRange Range(this Complex[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            var min = array[0].Magnitude;
            var max = array[0].Magnitude;

            for (var i = 1; i < array.Length; i++)
            {
                var value = array[i].Magnitude;

                if (min > value)
                    min = value;
                if (max < value)
                    max = value;
            }

            return new NumericRange(
                Math.Sqrt(min),
                Math.Sqrt(max));
        }

        /// <summary>
        ///     Compares two matrices for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this Complex[][] objA, Complex[][] objB, double threshold)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (var i = 0; i < objA.Length; i++)
                for (var j = 0; j < objA[i].Length; j++)
                {
                    var xr = objA[i][j].Real;
                    var yr = objB[i][j].Real;
                    var xi = objA[i][j].Imaginary;
                    var yi = objB[i][j].Imaginary;

                    if (Math.Abs(xr - yr) > threshold || double.IsNaN(xr) ^ double.IsNaN(yr))
                        return false;

                    if (Math.Abs(xi - yi) > threshold || double.IsNaN(xr) ^ double.IsNaN(yr))
                        return false;
                }

            return true;
        }

        /// <summary>
        ///     Compares two vectors for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this Complex[] objA, Complex[] objB, double threshold)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (var i = 0; i < objA.Length; i++)
            {
                var xr = objA[i].Real;
                var yr = objB[i].Real;
                var xi = objA[i].Imaginary;
                var yi = objB[i].Imaginary;

                if (Math.Abs(xr - yr) > threshold || double.IsNaN(xr) ^ double.IsNaN(yr))
                    return false;

                if (Math.Abs(xi - yi) > threshold || double.IsNaN(xi) ^ double.IsNaN(yi))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Gets the squared magnitude of a complex number.
        /// </summary>
        public static double SquaredMagnitude(this Complex value)
        {
            return value.Magnitude * value.Magnitude;
        }
    }
}