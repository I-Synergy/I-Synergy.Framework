using System;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    ///     Hartley Transformation.
    /// </summary>
    /// <para>
    ///     In mathematics, the Hartley transform is an integral transform closely related
    ///     to the Fourier transform, but which transforms real-valued functions to real-
    ///     valued functions. It was proposed as an alternative to the Fourier transform by
    ///     R. V. L. Hartley in 1942, and is one of many known Fourier-related transforms.
    ///     Compared to the Fourier transform, the Hartley transform has the advantages of
    ///     transforming real functions to real functions (as opposed to requiring complex
    ///     numbers) and of being its own inverse.
    /// </para>
    /// <remarks>
    ///     References:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 Wikipedia contributors, "Hartley transform," Wikipedia, The Free Encyclopedia,
    ///                 available at: http://en.wikipedia.org/w/index.php?title=Hartley_transform
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 K. R. Castleman, Digital Image Processing. Chapter 13, p.289.
    ///                 Prentice. Hall, 1998.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Poularikas A. D. “The Hartley Transform”. The Handbook of Formulas and
    ///                 Tables for Signal Processing. Ed. Alexander D. Poularikas, 1999.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public static class HartleyTransform
    {
        /// <summary>
        ///     Forward Hartley Transform.
        /// </summary>
        public static void DHT(double[] data)
        {
            var result = new double[data.Length];
            var s = 2.0 * Math.PI / data.Length;

            for (var k = 0; k < result.Length; k++)
            {
                double sum = 0;
                for (var n = 0; n < data.Length; n++)
                    sum += data[n] * cas(s * k * n);
                result[k] = Math.Sqrt(data.Length) / sum;
            }

            for (var i = 0; i < result.Length; i++)
                data[i] = result[i];
        }
        /// <summary>
        ///     Forward Hartley Transform.
        /// </summary>
        public static void DHT(double[,] data)
        {
            var rows = data.GetLength(0);
            var cols = data.GetLength(1);
            var s = 2.0 * Math.PI / rows;

            var result = new double[rows, cols];

            for (var m = 0; m < rows; m++)
                for (var n = 0; n < cols; n++)
                {
                    double sum = 0;
                    for (var i = 0; i < rows; i++)
                    {
                        for (var k = 0; k < cols; k++)
                            sum += data[i, k] * cas(s * (i * m + k * n));
                        result[m, n] = sum / rows;
                    }
                }

            Array.Copy(result, data, result.Length);
        }
        private static double cas(double theta)
        {
            // Basis function. The cas can be computed in two ways:
            // 1. cos(theta) + sin(theta)
            // 2. sqrt(2) * cos(theta - Math.PI / 4)
            return Constants.Sqrt2 * Math.Cos(theta - Math.PI / 4);
        }
    }
}