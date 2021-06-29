namespace ISynergy.Framework.Mathematics
{
    using System;
    using ISynergy.Framework.Mathematics;
    using System.Runtime.CompilerServices;

    public static partial class Elementwise
    {
#pragma warning disable 1591



        #region Matrix matrix

        /// <summary>
        ///   Multiplies a matrix <c>A</c> with a scalar <c>b</c> and accumulates with <c>c</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="c">The matrix <c>c</c>.</param>
        /// <param name="result">The matrix where the result should be stored. Pass the same
        ///   matrix as one of the arguments to perform the operation in place.</param>
        /// 
        public static int[][] MultiplyAndAdd(this int[][] a, int b, int[][] c, int[][] result)
        {
            check<int, int, int, int>(a: a, b: b, c: c, result: result);
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (int)((int)(a[i][j]) * b + (int)(c[i][j]));

            return result;
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> with a scalar <c>b</c> and accumulates with <c>c</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="c">The matrix <c>c</c>.</param>
        /// <param name="result">The matrix where the result should be stored. Pass the same
        ///   matrix as one of the arguments to perform the operation in place.</param>
        /// 
        public static int[,] MultiplyAndAdd(this int[,] a, int b, int[,] c, int[,] result)
        {
            check<int, int, int, int>(a: a, b: b, c: c, result: result);
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = (int)((int)(a[i, j]) * b + (int)(c[i, j]));

            return result;
        }

        #endregion



        #region Matrix matrix

        /// <summary>
        ///   Multiplies a matrix <c>A</c> with a scalar <c>b</c> and accumulates with <c>c</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="c">The matrix <c>c</c>.</param>
        /// <param name="result">The matrix where the result should be stored. Pass the same
        ///   matrix as one of the arguments to perform the operation in place.</param>
        /// 
        public static float[][] MultiplyAndAdd(this float[][] a, float b, float[][] c, float[][] result)
        {
            check<float, float, float, float>(a: a, b: b, c: c, result: result);
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (float)((float)(a[i][j]) * b + (float)(c[i][j]));

            return result;
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> with a scalar <c>b</c> and accumulates with <c>c</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="c">The matrix <c>c</c>.</param>
        /// <param name="result">The matrix where the result should be stored. Pass the same
        ///   matrix as one of the arguments to perform the operation in place.</param>
        /// 
        public static float[,] MultiplyAndAdd(this float[,] a, float b, float[,] c, float[,] result)
        {
            check<float, float, float, float>(a: a, b: b, c: c, result: result);
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = (float)((float)(a[i, j]) * b + (float)(c[i, j]));

            return result;
        }

        #endregion



        #region Matrix matrix

        /// <summary>
        ///   Multiplies a matrix <c>A</c> with a scalar <c>b</c> and accumulates with <c>c</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="c">The matrix <c>c</c>.</param>
        /// <param name="result">The matrix where the result should be stored. Pass the same
        ///   matrix as one of the arguments to perform the operation in place.</param>
        /// 
        public static double[][] MultiplyAndAdd(this double[][] a, double b, double[][] c, double[][] result)
        {
            check<double, double, double, double>(a: a, b: b, c: c, result: result);
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (double)((double)(a[i][j]) * b + (double)(c[i][j]));

            return result;
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> with a scalar <c>b</c> and accumulates with <c>c</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="c">The matrix <c>c</c>.</param>
        /// <param name="result">The matrix where the result should be stored. Pass the same
        ///   matrix as one of the arguments to perform the operation in place.</param>
        /// 
        public static double[,] MultiplyAndAdd(this double[,] a, double b, double[,] c, double[,] result)
        {
            check<double, double, double, double>(a: a, b: b, c: c, result: result);
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = (double)((double)(a[i, j]) * b + (double)(c[i, j]));

            return result;
        }

        #endregion



        #region Matrix matrix

        /// <summary>
        ///   Multiplies a matrix <c>A</c> with a scalar <c>b</c> and accumulates with <c>c</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="c">The matrix <c>c</c>.</param>
        /// <param name="result">The matrix where the result should be stored. Pass the same
        ///   matrix as one of the arguments to perform the operation in place.</param>
        /// 
        public static byte[][] MultiplyAndAdd(this byte[][] a, byte b, byte[][] c, byte[][] result)
        {
            check<byte, byte, byte, byte>(a: a, b: b, c: c, result: result);
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (byte)((byte)(a[i][j]) * b + (byte)(c[i][j]));

            return result;
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> with a scalar <c>b</c> and accumulates with <c>c</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="c">The matrix <c>c</c>.</param>
        /// <param name="result">The matrix where the result should be stored. Pass the same
        ///   matrix as one of the arguments to perform the operation in place.</param>
        /// 
        public static byte[,] MultiplyAndAdd(this byte[,] a, byte b, byte[,] c, byte[,] result)
        {
            check<byte, byte, byte, byte>(a: a, b: b, c: c, result: result);
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = (byte)((byte)(a[i, j]) * b + (byte)(c[i, j]));

            return result;
        }

        #endregion



        #region Matrix matrix

        /// <summary>
        ///   Multiplies a matrix <c>A</c> with a scalar <c>b</c> and accumulates with <c>c</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="c">The matrix <c>c</c>.</param>
        /// <param name="result">The matrix where the result should be stored. Pass the same
        ///   matrix as one of the arguments to perform the operation in place.</param>
        /// 
        public static decimal[][] MultiplyAndAdd(this decimal[][] a, decimal b, decimal[][] c, decimal[][] result)
        {
            check<decimal, decimal, decimal, decimal>(a: a, b: b, c: c, result: result);
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (decimal)((decimal)(a[i][j]) * b + (decimal)(c[i][j]));

            return result;
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> with a scalar <c>b</c> and accumulates with <c>c</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="c">The matrix <c>c</c>.</param>
        /// <param name="result">The matrix where the result should be stored. Pass the same
        ///   matrix as one of the arguments to perform the operation in place.</param>
        /// 
        public static decimal[,] MultiplyAndAdd(this decimal[,] a, decimal b, decimal[,] c, decimal[,] result)
        {
            check<decimal, decimal, decimal, decimal>(a: a, b: b, c: c, result: result);
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = (decimal)((decimal)(a[i, j]) * b + (decimal)(c[i, j]));

            return result;
        }

        #endregion


#pragma warning restore 1591
    }
}
