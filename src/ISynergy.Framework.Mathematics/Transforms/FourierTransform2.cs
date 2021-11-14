namespace ISynergy.Framework.Mathematics.Transforms
{
    /// <summary>
    ///     Fourier Transform (for arbitrary size matrices).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The transforms in this class accept arbitrary-length matrices and are not restricted to
    ///         only matrices that have dimensions which are powers of two. It also provides results which
    ///         are more equivalent with other mathematical packages, such as MATLAB and Octave.
    ///     </para>
    ///     <para>
    ///         This class had been created as an alternative to
    ///         <see cref="FourierTransform">
    ///             AForge.NET's
    ///             original FourierTransform class
    ///         </see>
    ///         that would provide more expected results.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <para>
    ///         The following examples show how to compute 1-D Discrete Fourier Transform and
    ///         1-D Fast Fourier Transforms, respectively:
    ///     </para>
    ///     <para>
    ///         The next examples show how to compute 2-D Discrete Fourier Transform and
    ///         2-D Fast Fourier Transforms, respectively:
    ///     </para>
    /// </example>
    /// <seealso cref="FourierTransform" />
    public static class FourierTransform2
    {
        // Trigonometric tables cached.
        private static double[] cosTable;
        private static double[] sinTable;
        private static double[] expCosTable;
        private static double[] expSinTable;

        /// <summary>
        ///     1-D Discrete Fourier Transform.
        /// </summary>
        /// <param name="data">The data to transform.</param>
        /// <param name="direction">The transformation direction.</param>
        public static void DFT(Complex[] data, FourierTransform.Direction direction)
        {
            var n = data.Length;
            var c = new Complex[n];

            // for each destination element
            for (var i = 0; i < c.Length; i++)
            {
                double sumRe = 0;
                double sumIm = 0;
                var phim = 2 * Math.PI * i / n;

                // sum source elements
                for (var j = 0; j < n; j++)
                {
                    var re = data[j].Real;
                    var im = data[j].Imaginary;
                    var cosw = Math.Cos(phim * j);
                    var sinw = Math.Sin(phim * j);

                    if (direction == FourierTransform.Direction.Backward)
                        sinw = -sinw;

                    sumRe += re * cosw + im * sinw;
                    sumIm += im * cosw - re * sinw;
                }

                c[i] = new Complex(sumRe, sumIm);
            }

            if (direction == FourierTransform.Direction.Backward)
                for (var i = 0; i < c.Length; i++)
                    data[i] = c[i] / n;
            else
                for (var i = 0; i < c.Length; i++)
                    data[i] = c[i];
        }

        /// <summary>
        ///     2-D Discrete Fourier Transform.
        /// </summary>
        /// <param name="data">The data to transform.</param>
        /// <param name="direction">The transformation direction.</param>
        public static void DFT2(Complex[][] data, FourierTransform.Direction direction)
        {
            var m = data.Columns();

            if (direction == FourierTransform.Direction.Forward)
            {
                // process rows
                for (var i = 0; i < data.Length; i++)
                    // transform it
                    DFT(data[i], FourierTransform.Direction.Forward);

                // process columns
                var col = new Complex[data.Length];
                for (var j = 0; j < m; j++)
                {
                    // copy column
                    for (var i = 0; i < col.Length; i++)
                        col[i] = data[i][j];

                    // transform it
                    DFT(col, FourierTransform.Direction.Forward);

                    // copy back
                    for (var i = 0; i < col.Length; i++)
                        data[i][j] = col[i];
                }
            }
            else
            {
                // process columns
                var col = new Complex[data.Length];
                for (var j = 0; j < m; j++)
                {
                    // copy column
                    for (var i = 0; i < col.Length; i++)
                        col[i] = data[i][j];

                    // transform it
                    DFT(col, FourierTransform.Direction.Backward);

                    // copy back
                    for (var i = 0; i < col.Length; i++)
                        data[i][j] = col[i];
                }

                // process rows
                for (var i = 0; i < data.Length; i++)
                    // transform it
                    DFT(data[i], FourierTransform.Direction.Backward);
            }
        }

        /// <summary>
        ///     1-D Fast Fourier Transform.
        /// </summary>
        /// <param name="data">The data to transform..</param>
        /// <param name="direction">The transformation direction.</param>
        public static void FFT(Complex[] data, FourierTransform.Direction direction)
        {
            var n = data.Length;

            if (n == 0)
                return;

            if (direction == FourierTransform.Direction.Backward)
                for (var i = 0; i < data.Length; i++)
                    data[i] = new Complex(data[i].Imaginary, data[i].Real);

            if ((n & (n - 1)) == 0)
                // Is power of 2
                TransformRadix2(data);
            else
                // More complicated algorithm for arbitrary sizes
                TransformBluestein(data);

            if (direction == FourierTransform.Direction.Backward)
                for (var i = 0; i < data.Length; i++)
                {
                    var im = data[i].Imaginary;
                    var re = data[i].Real;
                    data[i] = new Complex(im / n, re / n);
                }
        }

        /// <summary>
        ///     1-D Fast Fourier Transform.
        /// </summary>
        /// <param name="real">The real part of the complex numbers to transform.</param>
        /// <param name="imag">The imaginary part of the complex numbers to transform.</param>
        /// <param name="direction">The transformation direction.</param>
        public static void FFT(double[] real, double[] imag, FourierTransform.Direction direction)
        {
            if (direction == FourierTransform.Direction.Forward)
                FFT(real, imag);
            else
                FFT(imag, real);

            if (direction == FourierTransform.Direction.Backward)
                for (var i = 0; i < real.Length; i++)
                {
                    real[i] /= real.Length;
                    imag[i] /= real.Length;
                }
        }

        /// <summary>
        ///     2-D Fast Fourier Transform.
        /// </summary>
        /// <param name="data">The data to transform.</param>
        /// <param name="direction">The Transformation direction.</param>
        public static void FFT2(Complex[][] data, FourierTransform.Direction direction)
        {
            var n = data.Length;
            var m = data[0].Length;

            // process rows
            for (var i = 0; i < data.Length; i++)
                // transform it
                FFT(data[i], direction);

            // process columns
            var col = new Complex[n];
            for (var j = 0; j < m; j++)
            {
                // copy column
                for (var i = 0; i < col.Length; i++)
                    col[i] = data[i][j];

                // transform it
                FFT(col, direction);

                // copy back
                for (var i = 0; i < col.Length; i++)
                    data[i][j] = col[i];
            }
        }

        /// <summary>
        ///     Computes the discrete Fourier transform (DFT) of the given complex vector,
        ///     storing the result back into the vector. The vector can have any length.
        ///     This is a wrapper function.
        /// </summary>
        /// <param name="real">The real.</param>
        /// <param name="imag">The imag.</param>
        private static void FFT(double[] real, double[] imag)
        {
            var n = real.Length;

            if (n == 0)
                return;

            if ((n & (n - 1)) == 0)
                // Is power of 2
                TransformRadix2(real, imag);
            else
                // More complicated algorithm for arbitrary sizes
                TransformBluestein(real, imag);
        }

        /// <summary>
        ///     Computes the inverse discrete Fourier transform (IDFT) of the given complex
        ///     vector, storing the result back into the vector. The vector can have any length.
        ///     This is a wrapper function. This transform does not perform scaling, so the
        ///     inverse is not a true inverse.
        /// </summary>
        private static void IDFT(Complex[] data)
        {
            var n = data.Length;

            if (n == 0)
                return;

            for (var i = 0; i < data.Length; i++)
                data[i] = new Complex(data[i].Imaginary, data[i].Real);

            if ((n & (n - 1)) == 0)
                // Is power of 2
                TransformRadix2(data);
            else
                // More complicated algorithm for arbitrary sizes
                TransformBluestein(data);

            for (var i = 0; i < data.Length; i++)
            {
                var im = data[i].Imaginary;
                var re = data[i].Real;
                data[i] = new Complex(im, re);
            }
        }
        /// <summary>
        ///     Computes the inverse discrete Fourier transform (IDFT) of the given complex
        ///     vector, storing the result back into the vector. The vector can have any length.
        ///     This is a wrapper function. This transform does not perform scaling, so the
        ///     inverse is not a true inverse.
        /// </summary>
        private static void IDFT(double[] real, double[] imag)
        {
            FFT(imag, real);
        }

        /// <summary>
        ///     Computes the discrete Fourier transform (DFT) of the given complex vector, storing
        ///     the result back into the vector. The vector's length must be a power of 2. Uses the
        ///     Cooley-Tukey decimation-in-time radix-2 algorithm.
        /// </summary>
        /// <exception cref="System.ArgumentException">Length is not a power of 2.</exception>
        private static void TransformRadix2(double[] real, double[] imag)
        {
            var n = real.Length;

            var levels = (int)Math.Floor(Math.Log(n, 2));

            if (1 << levels != n)
                throw new ArgumentException("Length is not a power of 2");

            // Trigonometric tables.
            var cosTable = CosTable(n / 2);
            var sinTable = SinTable(n / 2);

            // Bit-reversed addressing permutation
            for (var i = 0; i < real.Length; i++)
            {
                var j = unchecked((int)((uint)Reverse(i) >> (32 - levels)));

                if (j > i)
                {
                    var temp = real[i];
                    real[i] = real[j];
                    real[j] = temp;

                    temp = imag[i];
                    imag[i] = imag[j];
                    imag[j] = temp;
                }
            }

            // Cooley-Tukey decimation-in-time radix-2 FFT
            for (int size = 2; size <= n; size *= 2)
            {
                var halfsize = size / 2;
                var tablestep = n / size;

                for (int i = 0; i < n; i += size)
                    for (int j = i, k = 0; j < i + halfsize; j++, k += tablestep)
                    {
                        var h = j + halfsize;
                        var re = real[h];
                        var im = imag[h];

                        var tpre = +re * cosTable[k] + im * sinTable[k];
                        var tpim = -re * sinTable[k] + im * cosTable[k];

                        real[h] = real[j] - tpre;
                        imag[h] = imag[j] - tpim;

                        real[j] += tpre;
                        imag[j] += tpim;
                    }

                // Prevent overflow in 'size *= 2'
                if (size == n)
                    break;
            }
        }

        /// <summary>
        ///     Computes the discrete Fourier transform (DFT) of the given complex vector, storing
        ///     the result back into the vector. The vector's length must be a power of 2. Uses the
        ///     Cooley-Tukey decimation-in-time radix-2 algorithm.
        /// </summary>
        /// <exception cref="System.ArgumentException">Length is not a power of 2.</exception>
        private static void TransformRadix2(Complex[] complex)
        {
            var n = complex.Length;

            var levels = (int)Math.Floor(Math.Log(n, 2));

            if (1 << levels != n)
                throw new ArgumentException("Length is not a power of 2");

            // Trigonometric tables.
            var cosTable = CosTable(n / 2);
            var sinTable = SinTable(n / 2);

            // Bit-reversed addressing permutation
            for (var i = 0; i < complex.Length; i++)
            {
                var j = unchecked((int)((uint)Reverse(i) >> (32 - levels)));

                if (j > i)
                {
                    var temp = complex[i];
                    complex[i] = complex[j];
                    complex[j] = temp;
                }
            }
            // Cooley-Tukey decimation-in-time radix-2 FFT
            for (var size = 2; size <= n; size *= 2)
            {
                var halfsize = size / 2;
                var tablestep = n / size;

                for (var i = 0; i < n; i += size)
                    for (int j = i, k = 0; j < i + halfsize; j++, k += tablestep)
                    {
                        var h = j + halfsize;
                        var re = complex[h].Real;
                        var im = complex[h].Imaginary;

                        var tpre = +re * cosTable[k] + im * sinTable[k];
                        var tpim = -re * sinTable[k] + im * cosTable[k];

                        var rej = complex[j].Real;
                        var imj = complex[j].Imaginary;

                        complex[h] = new Complex(rej - tpre, imj - tpim);
                        complex[j] = new Complex(rej + tpre, imj + tpim);
                    }

                // Prevent overflow in 'size *= 2'
                if (size == n)
                    break;
            }
        }
        /// <summary>
        ///     Computes the discrete Fourier transform (DFT) of the given complex vector, storing
        ///     the result back into the vector. The vector can have any length. This requires the
        ///     convolution function, which in turn requires the radix-2 FFT function. Uses
        ///     Bluestein's chirp z-transform algorithm.
        /// </summary>
        private static void TransformBluestein(double[] real, double[] imag)
        {
            var n = real.Length;
            var m = HighestOneBit(n * 2 + 1) << 1;

            // Trigonometric tables.
            var cosTable = ExpCosTable(n);
            var sinTable = ExpSinTable(n);

            // Temporary vectors and preprocessing
            var areal = new double[m];
            var aimag = new double[m];
            for (var i = 0; i < real.Length; i++)
            {
                areal[i] = +real[i] * cosTable[i] + imag[i] * sinTable[i];
                aimag[i] = -real[i] * sinTable[i] + imag[i] * cosTable[i];
            }

            var breal = new double[m];
            var bimag = new double[m];
            breal[0] = cosTable[0];
            bimag[0] = sinTable[0];

            for (var i = 1; i < cosTable.Length; i++)
            {
                breal[i] = breal[m - i] = cosTable[i];
                bimag[i] = bimag[m - i] = sinTable[i];
            }

            // Convolution
            var creal = new double[m];
            var cimag = new double[m];
            Convolve(areal, aimag, breal, bimag, creal, cimag);

            // Postprocessing
            for (var i = 0; i < n; i++)
            {
                real[i] = +creal[i] * cosTable[i] + cimag[i] * sinTable[i];
                imag[i] = -creal[i] * sinTable[i] + cimag[i] * cosTable[i];
            }
        }

        private static void TransformBluestein(Complex[] data)
        {
            var n = data.Length;
            var m = HighestOneBit(n * 2 + 1) << 1;

            // Trigonometric tables.
            var cosTable = ExpCosTable(n);
            var sinTable = ExpSinTable(n);

            // Temporary vectors and preprocessing
            var areal = new double[m];
            var aimag = new double[m];

            for (var i = 0; i < data.Length; i++)
            {
                var re = data[i].Real;
                var im = data[i].Imaginary;

                areal[i] = +re * cosTable[i] + im * sinTable[i];
                aimag[i] = -re * sinTable[i] + im * cosTable[i];
            }

            var breal = new double[m];
            var bimag = new double[m];
            breal[0] = cosTable[0];
            bimag[0] = sinTable[0];

            for (var i = 1; i < cosTable.Length; i++)
            {
                breal[i] = breal[m - i] = cosTable[i];
                bimag[i] = bimag[m - i] = sinTable[i];
            }

            // Convolution
            var creal = new double[m];
            var cimag = new double[m];
            Convolve(areal, aimag, breal, bimag, creal, cimag);

            // Postprocessing
            for (var i = 0; i < data.Length; i++)
            {
                var re = +creal[i] * cosTable[i] + cimag[i] * sinTable[i];
                var im = -creal[i] * sinTable[i] + cimag[i] * cosTable[i];
                data[i] = new Complex(re, im);
            }
        }

        /// <summary>
        ///     Computes the circular convolution of the given real
        ///     vectors. All vectors must have the same length.
        /// </summary>
        public static void Convolve(double[] x, double[] y, double[] result)
        {
            var n = x.Length;
            Convolve(x, new double[n], y, new double[n], result, new double[n]);
        }

        /// <summary>
        ///     Computes the circular convolution of the given complex
        ///     vectors. All vectors must have the same length.
        /// </summary>
        public static void Convolve(Complex[] x, Complex[] y, Complex[] result)
        {
            FFT(x, FourierTransform.Direction.Forward);
            FFT(y, FourierTransform.Direction.Forward);

            for (var i = 0; i < x.Length; i++)
            {
                var xreal = x[i].Real;
                var ximag = x[i].Imaginary;
                var yreal = y[i].Real;
                var yimag = y[i].Imaginary;

                var re = xreal * yreal - ximag * yimag;
                var im = ximag * yreal + xreal * yimag;

                x[i] = new Complex(re, im);
            }

            IDFT(x);

            // Scaling (because this FFT implementation omits it)
            for (var i = 0; i < x.Length; i++) result[i] = x[i] / x.Length;
        }

        /// <summary>
        ///     Computes the circular convolution of the given complex
        ///     vectors. All vectors must have the same length.
        /// </summary>
        public static void Convolve(double[] xreal, double[] ximag, double[] yreal, double[] yimag, double[] outreal,
            double[] outimag)
        {
            var n = xreal.Length;

            FFT(xreal, ximag);
            FFT(yreal, yimag);

            for (var i = 0; i < xreal.Length; i++)
            {
                var temp = xreal[i] * yreal[i] - ximag[i] * yimag[i];
                ximag[i] = ximag[i] * yreal[i] + xreal[i] * yimag[i];
                xreal[i] = temp;
            }

            IDFT(xreal, ximag);

            // Scaling (because this FFT implementation omits it)
            for (var i = 0; i < n; i++)
            {
                outreal[i] = xreal[i] / n;
                outimag[i] = ximag[i] / n;
            }
        }

        private static int HighestOneBit(int i)
        {
            i |= i >> 1;
            i |= i >> 2;
            i |= i >> 4;
            i |= i >> 8;
            i |= i >> 16;
            return i - (int)((uint)i >> 1);
        }

        private static int Reverse(int i)
        {
            i = ((i & 0x55555555) << 1) | ((int)((uint)i >> 1) & 0x55555555);
            i = ((i & 0x33333333) << 2) | ((int)((uint)i >> 2) & 0x33333333);
            i = ((i & 0x0f0f0f0f) << 4) | ((int)((uint)i >> 4) & 0x0f0f0f0f);
            i = (i << 24) | ((i & 0xff00) << 8) |
                ((int)((uint)i >> 8) & 0xff00) | (int)((uint)i >> 24);
            return i;
        }

        /// <summary>
        ///     Computes the Magnitude spectrum of a complex signal.
        /// </summary>
        public static double[] GetMagnitudeSpectrum(Complex[] fft)
        {
            if (fft == null)
                throw new ArgumentNullException("fft");

            // assumes fft is symmetric

            // In a two-sided spectrum, half the energy is displayed at the positive frequency,
            // and half the energy is displayed at the negative frequency. Therefore, to convert
            // from a two-sided spectrum to a single-sided spectrum, discard the second half of
            // the array and multiply every point except for DC by two.

            var numUniquePts = (int)Math.Ceiling((fft.Length + 1) / 2.0);
            var mx = new double[numUniquePts];

            mx[0] = fft[0].Magnitude / fft.Length;
            for (var i = 0; i < numUniquePts; i++)
                mx[i] = fft[i].Magnitude * 2 / fft.Length;

            return mx;
        }

        /// <summary>
        ///     Computes the Power spectrum of a complex signal.
        /// </summary>
        public static double[] GetPowerSpectrum(Complex[] fft)
        {
            if (fft == null)
                throw new ArgumentNullException("fft");

            var n = (int)Math.Ceiling((fft.Length + 1) / 2.0);

            var mx = new double[n];

            mx[0] = fft[0].SquaredMagnitude() / fft.Length;

            for (var i = 1; i < n; i++)
                mx[i] = fft[i].SquaredMagnitude() * 2.0 / fft.Length;

            return mx;
        }

        /// <summary>
        ///     Computes the Phase spectrum of a complex signal.
        /// </summary>
        public static double[] GetPhaseSpectrum(Complex[] fft)
        {
            if (fft == null) throw new ArgumentNullException("fft");

            var n = (int)Math.Ceiling((fft.Length + 1) / 2.0);

            var mx = new double[n];

            for (var i = 0; i < n; i++)
                mx[i] = fft[i].Phase;

            return mx;
        }

        /// <summary>
        ///     Creates an evenly spaced frequency vector (assuming a symmetric FFT)
        /// </summary>
        public static double[] GetFrequencyVector(int length, int sampleRate)
        {
            var numUniquePts = (int)Math.Ceiling((length + 1) / 2.0);

            var freq = new double[numUniquePts];
            for (var i = 0; i < numUniquePts; i++)
                freq[i] = i * sampleRate / (double)length;

            return freq;
        }

        /// <summary>
        ///     Gets the spectral resolution for a signal of given sampling rate and number of samples.
        /// </summary>
        public static double GetSpectralResolution(int samplingRate, int samples)
        {
            return samplingRate / (double)samples;
        }

        /// <summary>
        ///     Gets the power Cepstrum for a complex signal.
        /// </summary>
        public static double[] GetPowerCepstrum(Complex[] signal)
        {
            if (signal == null)
                throw new ArgumentNullException("signal");

            FourierTransform.FFT(signal, FourierTransform.Direction.Backward);

            var logabs = new Complex[signal.Length];
            for (var i = 0; i < logabs.Length; i++)
                logabs[i] = new Complex(Math.Log(signal[i].Magnitude), 0);

            FourierTransform.FFT(logabs, FourierTransform.Direction.Forward);

            return logabs.Re();
        }

        /// <summary>
        ///     Gets a half period cosine table.
        ///     Keeps the results in memory and reuses if parameters are the same.
        /// </summary>
        private static double[] CosTable(int sampleCount)
        {
            // Return table from memory if period matches.
            if (cosTable != null && sampleCount == cosTable.Length)
                return cosTable;

            // Create a new table and keep in memory.
            cosTable = new double[sampleCount];
            for (var i = 0; i < sampleCount; i++) cosTable[i] = Math.Cos(Math.PI * i / sampleCount);
            return cosTable;
        }

        /// <summary>
        ///     Gets a half period sinus table.
        ///     Keeps the results in memory and reuses if parameters are the same.
        /// </summary>
        private static double[] SinTable(int sampleCount)
        {
            // Return table from memory if period matches.
            if (sinTable != null && sampleCount == sinTable.Length)
                return sinTable;

            // Create a new table and keep in memory.
            sinTable = new double[sampleCount];
            for (var i = 0; i < sampleCount; i++) sinTable[i] = Math.Sin(Math.PI * i / sampleCount);
            return sinTable;
        }

        /// <summary>
        ///     Gets a cosine table with exponentially increasing frequency by i * i.
        ///     Keeps the results in memory and reuses if parameters are the same.
        /// </summary>
        private static double[] ExpCosTable(int sampleCount)
        {
            // Return table from memory if period matches.
            if (expCosTable != null && sampleCount == expCosTable.Length)
                return expCosTable;

            // Create a new table and keep in memory.
            expCosTable = new double[sampleCount];
            for (var i = 0; i < sampleCount; i++)
            {
                var j = (int)((long)i * i % (sampleCount * 2)); // This is more accurate than j = i * i
                expCosTable[i] = Math.Cos(Math.PI * j / sampleCount);
            }

            return expCosTable;
        }

        /// <summary>
        ///     Gets a sinus table with exponentially increasing frequency by i * i.
        ///     Keeps the results in memory and reuses if parameters are the same.
        /// </summary>
        private static double[] ExpSinTable(int sampleCount)
        {
            // Return table from memory if period matches.
            if (expSinTable != null && sampleCount == expSinTable.Length)
                return expSinTable;

            // Create a new table and keep in memory.
            expSinTable = new double[sampleCount];
            for (var i = 0; i < sampleCount; i++)
            {
                var j = (int)((long)i * i % (sampleCount * 2)); // This is more accurate than j = i * i
                expSinTable[i] = Math.Sin(Math.PI * j / sampleCount);
            }

            return expSinTable;
        }
    }
}