using System.Diagnostics;
using System.Numerics;

namespace ISynergy.Framework.Mathematics.Functions
{
    /// <summary>
    ///     Gabor kernel types.
    /// </summary>
    public enum GaborKernelKind
    {
        /// <summary>
        ///     Creates kernel based on the real part of the Gabor function.
        /// </summary>
        Real,

        /// <summary>
        ///     Creates a kernel based on the imaginary part of the Gabor function.
        /// </summary>
        Imaginary,

        /// <summary>
        ///     Creates a kernel based on the Magnitude of the Gabor function.
        /// </summary>
        Magnitude,

        /// <summary>
        ///     Creates a kernel based on the Squared Magnitude of the Gabor function.
        /// </summary>
        SquaredMagnitude
    }

    /// <summary>
    ///     Gabor functions.
    /// </summary>
    /// <remarks>
    ///     This class has been contributed by Diego Catalano, author of the Catalano
    ///     Framework, a native port of AForge.NET and ISynergy.Framework.Mathematics.NET for Java and Android.
    /// </remarks>
    public static class Gabor
    {
        /// <summary>
        ///     1-D Gabor function.
        /// </summary>
        public static double Function1D(double x, double mean, double amplitude,
            double position, double width, double phase, double frequency)
        {
            var a = (x - position) * (x - position);
            var b = 2 * width * (2 * width);

            var envelope = mean + amplitude * Math.Exp(-a / b);
            var carry = Math.Cos(2 * Math.PI * frequency * (x - position) + phase);

            return envelope * carry;
        }

        /// <summary>
        ///     2-D Gabor function.
        /// </summary>
        public static Complex Function2D(double x, double y, double lambda, double theta,
            double psi, double sigma, double gamma)
        {
            var X = +x * Math.Cos(theta) + y * Math.Sin(theta);
            var Y = -x * Math.Sin(theta) + y * Math.Cos(theta);

            var envelope = Math.Exp(-((X * X + gamma * gamma * Y * Y) / (2 * sigma * sigma)));
            var real = Math.Cos(2 * Math.PI * (X / lambda) + psi);
            var imaginary = Math.Sin(2 * Math.PI * (X / lambda) + psi);

            return new Complex(envelope * real, envelope * imaginary);
        }

        /// <summary>
        ///     Real part of the 2-D Gabor function.
        /// </summary>
        public static double RealFunction2D(double x, double y, double lambda, double theta,
            double psi, double sigma, double gamma)
        {
            var X = +x * Math.Cos(theta) + y * Math.Sin(theta);
            var Y = -x * Math.Sin(theta) + y * Math.Cos(theta);

            var envelope = Math.Exp(-((X * X + gamma * gamma * Y * Y) / (2 * sigma * sigma)));
            var carrier = Math.Cos(2 * Math.PI * (X / lambda) + psi);

            return envelope * carrier;
        }

        /// <summary>
        ///     Imaginary part of the 2-D Gabor function.
        /// </summary>
        public static double ImaginaryFunction2D(double x, double y, double lambda, double theta,
            double psi, double sigma, double gamma)
        {
            var X = +x * Math.Cos(theta) + y * Math.Sin(theta);
            var Y = -x * Math.Sin(theta) + y * Math.Cos(theta);

            var envelope = Math.Exp(-((X * X + gamma * gamma * Y * Y) / (2 * sigma * sigma)));
            var carrier = Math.Sin(2 * Math.PI * (X / lambda) + psi);

            return envelope * carrier;
        }

        /// <summary>
        ///     Computes the 2-D Gabor kernel.
        /// </summary>
        public static double[,] Kernel2D(double lambda, double theta, double psi,
            double sigma, double gamma)
        {
            return Kernel2D(3, lambda, theta, psi, sigma, gamma, false, GaborKernelKind.Real);
        }

        /// <summary>
        ///     Computes the 2-D Gabor kernel.
        /// </summary>
        public static double[,] Kernel2D(double lambda, double theta, double psi,
            double sigma, double gamma, bool normalized)
        {
            return Kernel2D(3, lambda, theta, psi, sigma, gamma, normalized, GaborKernelKind.Imaginary);
        }

        /// <summary>
        ///     Computes the 2-D Gabor kernel.
        /// </summary>
        public static double[,] Kernel2D(int size, double lambda, double theta, double psi,
            double sigma, double gamma, bool normalized)
        {
            return Kernel2D(size, lambda, theta, psi, sigma,
                gamma, normalized, GaborKernelKind.Imaginary);
        }
        /// <summary>
        ///     Computes the 2-D Gabor kernel.
        /// </summary>
        public static double[,] Kernel2D(int size, double lambda, double theta,
            double psi, double sigma, double gamma, bool normalized, GaborKernelKind function)
        {
            var sigmaX = sigma;
            var sigmaY = sigma / gamma;

            var a = Math.Max(
                Math.Abs(size * sigmaX * Math.Cos(theta)),
                Math.Abs(size * sigmaY * Math.Sin(theta)));
            var xMax = (int)Math.Ceiling(Math.Max(1, a));

            var b = Math.Max(
                Math.Abs(size * sigmaX * Math.Sin(theta)),
                Math.Abs(size * sigmaY * Math.Cos(theta)));
            var yMax = (int)Math.Ceiling(Math.Max(1, b));

            var xValues = Vector.Interval(-xMax, xMax);
            var yValues = Vector.Interval(-yMax, yMax);

            Debug.Assert(xValues.Length == 2 * xMax + 1);
            Debug.Assert(yValues.Length == 2 * yMax + 1);

            var kernel = new double[xValues.Length, yValues.Length];

            double sum = 0;

            switch (function)
            {
                case GaborKernelKind.Real:
                    for (var i = 0; i < xValues.Length; i++)
                        for (var j = 0; j < yValues.Length; j++)
                            sum += kernel[i, j] = RealFunction2D(
                                xValues[i], yValues[j], lambda, theta, psi, sigma, gamma);
                    break;

                case GaborKernelKind.Imaginary:
                    for (var i = 0; i < xValues.Length; i++)
                        for (var j = 0; j < yValues.Length; j++)
                            sum += kernel[i, j] = ImaginaryFunction2D(
                                xValues[i], yValues[j], lambda, theta, psi, sigma, gamma);
                    break;

                case GaborKernelKind.Magnitude:
                    for (var i = 0; i < xValues.Length; i++)
                        for (var j = 0; j < yValues.Length; j++)
                            sum += kernel[i, j] = Function2D(
                                xValues[i], yValues[j], lambda, theta, psi, sigma, gamma).Magnitude;
                    break;

                case GaborKernelKind.SquaredMagnitude:
                    for (var i = 0; i < xValues.Length; i++)
                        for (var j = 0; j < yValues.Length; j++)
                        {
                            var v = Function2D(
                                xValues[i], yValues[j], lambda, theta, psi, sigma, gamma).Magnitude;
                            sum += kernel[i, j] = v * v;
                        }

                    break;
            }

            if (normalized)
                kernel.Divide(sum, result: kernel);

            return kernel;
        }
    }
}