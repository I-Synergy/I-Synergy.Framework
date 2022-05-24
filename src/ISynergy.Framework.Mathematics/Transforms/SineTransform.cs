using System;

namespace ISynergy.Framework.Mathematics.Transforms
{
    /// <summary>
    ///     Discrete Sine Transform
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         In mathematics, the discrete sine transform (DST) is a Fourier-related transform
    ///         similar to the discrete Fourier transform (DFT), but using a purely real matrix. It
    ///         is equivalent to the imaginary parts of a DFT of roughly twice the length, operating
    ///         on real data with odd symmetry (since the Fourier transform of a real and odd function
    ///         is imaginary and odd), where in some variants the input and/or output data are shifted
    ///         by half a sample.
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
    public static class SineTransform
    {
        /// <summary>
        ///     Forward Discrete Sine Transform.
        /// </summary>
        public static void DST(double[] data)
        {
            var result = new double[data.Length];

            for (var k = 1; k < result.Length + 1; k++)
            {
                double sum = 0;
                for (var i = 1; i < data.Length + 1; i++)
                    sum += data[i - 1] * Math.Sin(Math.PI * (k * i / (data.Length + 1.0)));
                result[k - 1] = sum;
            }

            for (var i = 0; i < data.Length; i++)
                data[i] = result[i];
        }

        /// <summary>
        ///     Inverse Discrete Sine Transform.
        /// </summary>
        public static void IDST(double[] data)
        {
            var result = new double[data.Length];

            var inverse = 2.0 / (data.Length + 1);

            for (var k = 1; k < result.Length + 1; k++)
            {
                double sum = 0;
                for (var i = 1; i < data.Length + 1; i++)
                    sum += data[i - 1] * Math.Sin(Math.PI * (k * i / (data.Length + 1.0)));
                result[k - 1] = sum * inverse;
            }

            for (var i = 0; i < data.Length; i++)
                data[i] = result[i];
        }

        /// <summary>
        ///     Forward Discrete Sine Transform.
        /// </summary>
        public static void DST(double[][] data)
        {
            var rows = data.Length;
            var cols = data[0].Length;

            var row = new double[cols];
            var col = new double[rows];

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < row.Length; j++)
                    row[j] = data[i][j];

                DST(row);

                for (var j = 0; j < row.Length; j++)
                    data[i][j] = row[j];
            }

            for (var j = 0; j < cols; j++)
            {
                for (var i = 0; i < col.Length; i++)
                    col[i] = data[i][j];

                DST(col);

                for (var i = 0; i < col.Length; i++)
                    data[i][j] = col[i];
            }
        }

        /// <summary>
        ///     Inverse Discrete Sine Transform.
        /// </summary>
        public static void IDST(double[][] data)
        {
            var rows = data.Length;
            var cols = data[0].Length;

            var row = new double[cols];
            var col = new double[rows];

            for (var j = 0; j < cols; j++)
            {
                for (var i = 0; i < row.Length; i++)
                    col[i] = data[i][j];

                IDST(col);

                for (var i = 0; i < col.Length; i++)
                    data[i][j] = col[i];
            }

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < row.Length; j++)
                    row[j] = data[i][j];

                IDST(row);

                for (var j = 0; j < row.Length; j++)
                    data[i][j] = row[j];
            }
        }
    }
}