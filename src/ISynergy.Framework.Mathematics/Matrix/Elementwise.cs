using ISynergy.Framework.Mathematics.Exceptions;

namespace ISynergy.Framework.Mathematics
{
    using System.CodeDom.Compiler;
    using System.Diagnostics;

    /// <summary>
    ///   Vector types.
    /// </summary>
    /// 
    public enum VectorType : int
    {
        /// <summary>
        ///   The vector is a row vector, meaning it should have a size equivalent 
        ///   to [1 x N] where N is the number of elements in the vector.
        /// </summary>
        /// 
        RowVector = 0,

        /// <summary>
        ///   The vector is a column vector, meaning it should have a size equivalent
        ///   to [N x 1] where N is the number of elements in the vector.
        /// </summary>
        /// 
        ColumnVector = 1
    }

    /// <summary>
    ///   Elementwise matrix and vector operations.
    /// </summary>
    /// 
    /// <seealso cref="VectorType"/>
    ///
    [GeneratedCode("ISynergy.Framework.Mathematics.NET T4 Templates", "3.7")]
    public static partial class Elementwise
    {
        private static int rows<U>(U[] b)
        {
            return b.Length;
        }

        private static int cols<U>(U[][] b)
        {
            return b[0].Length;
        }

        private static int rows<U>(U[,] b)
        {
            return b.GetLength(0);
        }

        private static int cols<U>(U[,] b)
        {
            return b.GetLength(1);
        }


        [Conditional("DEBUG")]
        static void check<T, U>(T[] a, U[] b)
        {
            if (a.Length != b.Length)
                throw new DimensionMismatchException("b");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[] a, U[] b, V[] result)
        {
            if (a.Length != b.Length)
                throw new DimensionMismatchException("b");
            if (a.Length != result.Length)
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[] a, U b, V[] result)
        {
            if (a.Length != result.Length)
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T a, U[] b, V[] result)
        {
            if (b.Length != result.Length)
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U>(T[,] a, U[,] b)
        {
            if (rows(a) != rows(b) || cols(a) != cols(b))
                throw new DimensionMismatchException("b");
        }

        [Conditional("DEBUG")]
        static void check<T, U>(T[][] a, U[][] b)
        {
            if (rows(a) != rows(b) || cols(a) != cols(b))
                throw new DimensionMismatchException("b");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[,] a, U[,] b, V[,] result)
        {
            if (rows(a) != rows(b) || cols(a) != cols(b))
                throw new DimensionMismatchException("b");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[][] a, U[][] b, V[][] result)
        {
            if (rows(a) != rows(b) || cols(a) != cols(b))
                throw new DimensionMismatchException("b");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }




        [Conditional("DEBUG")]
        static void check<T, U, V>(int d, T[,] a, U[] b, V[,] result)
        {
            if (d == 0)
            {
                if (cols(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(int d, T[][] a, U[] b, V[][] result)
        {
            if (d == 0)
            {
                if (cols(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }



        [Conditional("DEBUG")]
        static void check<T, U, V>(int d, T[] a, U[,] b, V[,] result)
        {
            if (d == 0)
            {
                if (cols(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(int d, T[] a, U[][] b, V[][] result)
        {
            if (d == 0)
            {
                if (cols(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }








        [Conditional("DEBUG")]
        static void check<T, U, V>(VectorType d, T[,] a, U[] b, V[,] result)
        {
            if (d == 0)
            {
                if (cols(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(VectorType d, T[][] a, U[] b, V[][] result)
        {
            if (d == 0)
            {
                if (cols(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }



        [Conditional("DEBUG")]
        static void check<T, U, V>(VectorType d, T[] a, U[,] b, V[,] result)
        {
            if (d == 0)
            {
                if (cols(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(VectorType d, T[] a, U[][] b, V[][] result)
        {
            if (d == 0)
            {
                if (cols(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }








        [Conditional("DEBUG")]
        static void check<T, U, V>(T[,] a, U[] b, V[,] result)
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException("b");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[][] a, U[] b, V[][] result)
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException("b");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[] a, U[,] b, V[,] result)
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException("b");
            if (rows(a) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[] a, U[][] b, V[][] result)
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException("b");
            if (rows(a) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }





        [Conditional("DEBUG")]
        static void check<T, U, V>(int d, T a, U[,] b, V[,] result)
        {
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(int d, T a, U[][] b, V[][] result)
        {
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T a, U[,] b, V[,] result)
        {
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T a, U[][] b, V[][] result)
        {
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }








        [Conditional("DEBUG")]
        static void check<T, U, V>(T[][] a, U b, V[][] result)
        {
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[,] a, U b, V[,] result)
        {
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V, W>(T[][] a, U b, V[,] c, W[,] result)
        {
            if (rows(a) != rows(c) || cols(a) != cols(c))
                throw new DimensionMismatchException("c");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V, W>(T[][] a, U b, V[][] c, W[][] result)
        {
            if (rows(a) != rows(c) || cols(a) != cols(c))
                throw new DimensionMismatchException("c");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V, W>(T[,] a, U b, V[,] c, W[,] result)
        {
            if (rows(a) != rows(c) || cols(a) != cols(c))
                throw new DimensionMismatchException("c");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

    }
}
