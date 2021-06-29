using System;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    ///     Discrete Cosine Transformation.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A discrete cosine transform (DCT) expresses a finite sequence of data points
    ///         in terms of a sum of cosine functions oscillating at different frequencies.
    ///         DCTs are important to numerous applications in science and engineering, from
    ///         lossy compression of audio (e.g. MP3) and images (e.g. JPEG) (where small
    ///         high-frequency components can be discarded), to spectral methods for the
    ///         numerical solution of partial differential equations.
    ///     </para>
    ///     <para>
    ///         The use of cosine rather than sine functions is critical in these applications:
    ///         for compression, it turns out that cosine functions are much more efficient,
    ///         whereas for differential equations the cosines express a particular choice of
    ///         boundary conditions.
    ///     </para>
    ///     <para>
    ///         References:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>
    ///                     Wikipedia contributors, "Discrete sine transform," Wikipedia, The Free Encyclopedia,
    ///                     available at: http://en.wikipedia.org/w/index.php?title=Discrete_sine_transform
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     K. R. Castleman, Digital Image Processing. Chapter 13, p.288.
    ///                     Prentice. Hall, 1998.
    ///                 </description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public static class CosineTransform
    {
        /// <summary>
        ///     Forward Discrete Cosine Transform.
        /// </summary>
        public static void DCT(double[] data)
        {
            var result = new double[data.Length];
            var c = Math.PI / (2.0 * data.Length);
            var scale = Math.Sqrt(2.0 / data.Length);

            for (var k = 0; k < data.Length; k++)
            {
                double sum = 0;
                for (var n = 0; n < data.Length; n++)
                    sum += data[n] * Math.Cos((2.0 * n + 1.0) * k * c);
                result[k] = scale * sum;
            }

            data[0] = result[0] / Constants.Sqrt2;
            for (var i = 1; i < data.Length; i++)
                data[i] = result[i];
        }

        /// <summary>
        ///     Inverse Discrete Cosine Transform.
        /// </summary>
        public static void IDCT(double[] data)
        {
            var result = new double[data.Length];
            var c = Math.PI / (2.0 * data.Length);
            var scale = Math.Sqrt(2.0 / data.Length);

            for (var k = 0; k < data.Length; k++)
            {
                var sum = data[0] / Constants.Sqrt2;
                for (var n = 1; n < data.Length; n++)
                    sum += data[n] * Math.Cos((2 * k + 1) * n * c);

                result[k] = scale * sum;
            }

            for (var i = 0; i < data.Length; i++)
                data[i] = result[i];
        }


        /// <summary>
        ///     Forward 2D Discrete Cosine Transform.
        /// </summary>
        public static void DCT(double[,] data)
        {
            var rows = data.GetLength(0);
            var cols = data.GetLength(1);

            var row = new double[cols];
            var col = new double[rows];

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < row.Length; j++)
                    row[j] = data[i, j];

                DCT(row);

                for (var j = 0; j < row.Length; j++)
                    data[i, j] = row[j];
            }

            for (var j = 0; j < cols; j++)
            {
                for (var i = 0; i < col.Length; i++)
                    col[i] = data[i, j];

                DCT(col);

                for (var i = 0; i < col.Length; i++)
                    data[i, j] = col[i];
            }
        }

        /// <summary>
        ///     Inverse 2D Discrete Cosine Transform.
        /// </summary>
        public static void IDCT(double[,] data)
        {
            var rows = data.GetLength(0);
            var cols = data.GetLength(1);

            var row = new double[cols];
            var col = new double[rows];

            for (var j = 0; j < cols; j++)
            {
                for (var i = 0; i < col.Length; i++)
                    col[i] = data[i, j];

                IDCT(col);

                for (var i = 0; i < col.Length; i++)
                    data[i, j] = col[i];
            }

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < row.Length; j++)
                    row[j] = data[i, j];

                IDCT(row);

                for (var j = 0; j < row.Length; j++)
                    data[i, j] = row[j];
            }
        }
    }
}