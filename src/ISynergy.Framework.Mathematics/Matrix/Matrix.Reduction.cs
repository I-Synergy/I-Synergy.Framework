namespace ISynergy.Framework.Mathematics
{
    public static partial class Matrix
    {
        /// <summary>
        ///   Vector sum.
        /// </summary>
        ///
        /// <param name="vector">A vector whose sum will be calculated.</param>
        ///
        public static double Sum(this double[] vector)
        {
            double sum = 0;
            for (var i = 0; i < vector.Length; i++)
                sum = (double)(sum + (double)vector[i]);
            return sum;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        ///
        public static double Sum(this double[,] matrix)
        {
            double sum = 0;
            foreach (var v in matrix)
                sum = (double)(sum + (double)v);
            return sum;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        ///
        public static double Sum(this double[][] matrix)
        {
            double sum = 0;
            for (var i = 0; i < matrix.Length; i++)
                for (var j = 0; j < matrix[i].Length; j++)
                    sum = (double)(sum + (double)matrix[i][j]);
            return sum;
        }
        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        ///
        public static double[] Sum(this double[][] matrix, int dimension)
        {
            var result = new double[Matrix.GetLength(matrix, dimension)];
            return Sum(matrix, dimension, result);
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        ///
        public static double[] Sum(this double[,] matrix, int dimension)
        {
            var result = new double[Matrix.GetLength(matrix, dimension)];
            return Sum(matrix, dimension, result);
        }
        /// <summary>
        ///   Vector product.
        /// </summary>
        ///
        /// <param name="vector">A vector whose product will be calculated.</param>
        ///
        public static double Product(this double[] vector)
        {
            double sum = 1;
            for (var i = 0; i < vector.Length; i++)
                sum = (double)(sum * (double)vector[i]);
            return sum;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        ///
        public static double Product(this double[,] matrix)
        {
            double sum = 1;
            foreach (var v in matrix)
                sum = (double)(sum * (double)v);
            return sum;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        ///
        public static double Product(this double[][] matrix)
        {
            double sum = 1;
            for (var i = 0; i < matrix.Length; i++)
                for (var j = 0; j < matrix[i].Length; j++)
                    sum = (double)(sum * (double)matrix[i][j]);
            return sum;
        }
        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        ///
        public static double[] Product(this double[][] matrix, int dimension)
        {
            var result = new double[Matrix.GetLength(matrix, dimension)];
            return Product(matrix, dimension, result);
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        ///
        public static double[] Product(this double[,] matrix, int dimension)
        {
            var result = new double[Matrix.GetLength(matrix, dimension)];
            return Product(matrix, dimension, result);
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static int[] Sum(this double[][] matrix, int dimension, int[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    int s = 0;
                    for (var i = 0; i < rows; i++)
                        s = (int)(s + (int)matrix[i][j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    int s = 0;
                    for (var i = 0; i < cols; i++)
                        s = (int)(s + (int)matrix[j][i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static int[] Sum(this double[,] matrix, int dimension, int[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    int s = 0;
                    for (var i = 0; i < rows; i++)
                        s = (int)(s + (int)matrix[i, j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    int s = 0;
                    for (var i = 0; i < cols; i++)
                        s = (int)(s + (int)matrix[j, i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static int[] Product(this double[][] matrix, int dimension, int[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    int s = 1;
                    for (var i = 0; i < rows; i++)
                        s = (int)(s * (int)matrix[i][j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    int s = 1;
                    for (var i = 0; i < cols; i++)
                        s = (int)(s * (int)matrix[j][i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static int[] Product(this double[,] matrix, int dimension, int[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    int s = 1;
                    for (var i = 0; i < rows; i++)
                        s = (int)(s * (int)matrix[i, j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    int s = 1;
                    for (var i = 0; i < cols; i++)
                        s = (int)(s * (int)matrix[j, i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static short[] Sum(this double[][] matrix, int dimension, short[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    short s = 0;
                    for (var i = 0; i < rows; i++)
                        s = (short)(s + (short)matrix[i][j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    short s = 0;
                    for (var i = 0; i < cols; i++)
                        s = (short)(s + (short)matrix[j][i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static short[] Sum(this double[,] matrix, int dimension, short[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    short s = 0;
                    for (var i = 0; i < rows; i++)
                        s = (short)(s + (short)matrix[i, j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    short s = 0;
                    for (var i = 0; i < cols; i++)
                        s = (short)(s + (short)matrix[j, i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static short[] Product(this double[][] matrix, int dimension, short[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    short s = 1;
                    for (var i = 0; i < rows; i++)
                        s = (short)(s * (short)matrix[i][j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    short s = 1;
                    for (var i = 0; i < cols; i++)
                        s = (short)(s * (short)matrix[j][i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static short[] Product(this double[,] matrix, int dimension, short[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    short s = 1;
                    for (var i = 0; i < rows; i++)
                        s = (short)(s * (short)matrix[i, j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    short s = 1;
                    for (var i = 0; i < cols; i++)
                        s = (short)(s * (short)matrix[j, i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static float[] Sum(this double[][] matrix, int dimension, float[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    float s = 0;
                    for (var i = 0; i < rows; i++)
                        s = (float)(s + (float)matrix[i][j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    float s = 0;
                    for (var i = 0; i < cols; i++)
                        s = (float)(s + (float)matrix[j][i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static float[] Sum(this double[,] matrix, int dimension, float[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    float s = 0;
                    for (var i = 0; i < rows; i++)
                        s = (float)(s + (float)matrix[i, j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    float s = 0;
                    for (var i = 0; i < cols; i++)
                        s = (float)(s + (float)matrix[j, i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static float[] Product(this double[][] matrix, int dimension, float[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    float s = 1;
                    for (var i = 0; i < rows; i++)
                        s = (float)(s * (float)matrix[i][j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    float s = 1;
                    for (var i = 0; i < cols; i++)
                        s = (float)(s * (float)matrix[j][i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static float[] Product(this double[,] matrix, int dimension, float[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    float s = 1;
                    for (var i = 0; i < rows; i++)
                        s = (float)(s * (float)matrix[i, j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    float s = 1;
                    for (var i = 0; i < cols; i++)
                        s = (float)(s * (float)matrix[j, i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static double[] Sum(this double[][] matrix, int dimension, double[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    double s = 0;
                    for (var i = 0; i < rows; i++)
                        s = (double)(s + (double)matrix[i][j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    double s = 0;
                    for (var i = 0; i < cols; i++)
                        s = (double)(s + (double)matrix[j][i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static double[] Sum(this double[,] matrix, int dimension, double[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    double s = 0;
                    for (var i = 0; i < rows; i++)
                        s = (double)(s + (double)matrix[i, j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    double s = 0;
                    for (var i = 0; i < cols; i++)
                        s = (double)(s + (double)matrix[j, i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static double[] Product(this double[][] matrix, int dimension, double[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    double s = 1;
                    for (var i = 0; i < rows; i++)
                        s = (double)(s * (double)matrix[i][j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    double s = 1;
                    for (var i = 0; i < cols; i++)
                        s = (double)(s * (double)matrix[j][i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static double[] Product(this double[,] matrix, int dimension, double[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    double s = 1;
                    for (var i = 0; i < rows; i++)
                        s = (double)(s * (double)matrix[i, j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    double s = 1;
                    for (var i = 0; i < cols; i++)
                        s = (double)(s * (double)matrix[j, i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static long[] Sum(this double[][] matrix, int dimension, long[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    long s = 0;
                    for (var i = 0; i < rows; i++)
                        s = (long)(s + (long)matrix[i][j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    long s = 0;
                    for (var i = 0; i < cols; i++)
                        s = (long)(s + (long)matrix[j][i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static long[] Sum(this double[,] matrix, int dimension, long[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    long s = 0;
                    for (var i = 0; i < rows; i++)
                        s = (long)(s + (long)matrix[i, j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    long s = 0;
                    for (var i = 0; i < cols; i++)
                        s = (long)(s + (long)matrix[j, i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static long[] Product(this double[][] matrix, int dimension, long[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    long s = 1;
                    for (var i = 0; i < rows; i++)
                        s = (long)(s * (long)matrix[i][j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    long s = 1;
                    for (var i = 0; i < cols; i++)
                        s = (long)(s * (long)matrix[j][i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static long[] Product(this double[,] matrix, int dimension, long[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    long s = 1;
                    for (var i = 0; i < rows; i++)
                        s = (long)(s * (long)matrix[i, j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    long s = 1;
                    for (var i = 0; i < cols; i++)
                        s = (long)(s * (long)matrix[j, i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static decimal[] Sum(this double[][] matrix, int dimension, decimal[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    decimal s = 0;
                    for (var i = 0; i < rows; i++)
                        s = (decimal)(s + (decimal)matrix[i][j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    decimal s = 0;
                    for (var i = 0; i < cols; i++)
                        s = (decimal)(s + (decimal)matrix[j][i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static decimal[] Sum(this double[,] matrix, int dimension, decimal[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    decimal s = 0;
                    for (var i = 0; i < rows; i++)
                        s = (decimal)(s + (decimal)matrix[i, j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    decimal s = 0;
                    for (var i = 0; i < cols; i++)
                        s = (decimal)(s + (decimal)matrix[j, i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static decimal[] Product(this double[][] matrix, int dimension, decimal[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    decimal s = 1;
                    for (var i = 0; i < rows; i++)
                        s = (decimal)(s * (decimal)matrix[i][j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    decimal s = 1;
                    for (var i = 0; i < cols; i++)
                        s = (decimal)(s * (decimal)matrix[j][i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix product.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose product will be calculated.</param>
        /// <param name="dimension">The dimension in which the product will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static decimal[] Product(this double[,] matrix, int dimension, decimal[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (dimension == 0)
            {
                for (var j = 0; j < cols; j++)
                {
                    decimal s = 1;
                    for (var i = 0; i < rows; i++)
                        s = (decimal)(s * (decimal)matrix[i, j]);
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (var j = 0; j < rows; j++)
                {
                    decimal s = 1;
                    for (var i = 0; i < cols; i++)
                        s = (decimal)(s * (decimal)matrix[j, i]);
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix cumulative sum.
        /// </summary>
        ///
        /// <param name="vector">A vector whose cumulative sum will be calculated.</param>
        ///
        public static double[] CumulativeSum(this double[] vector)
        {
            if (vector.Length == 0)
                return new double[0];

            return CumulativeSum(vector, ISynergy.Framework.Mathematics.Vector.CreateAs(vector));
        }

        /// <summary>
        ///   Matrix cumulative sum.
        /// </summary>
        ///
        /// <param name="vector">A vector whose cumulative sum will be calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static double[] CumulativeSum(this double[] vector, double[] result)
        {
            result[0] = vector[0];
            for (var i = 1; i < vector.Length; i++)
                result[i] = (double)(result[i - 1] + vector[i]);
            return result;
        }

        /// <summary>
        ///   Matrix cumulative sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose cumulative sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the cumulative will be
        ///   calculated.</param>
        ///
        public static double[][] CumulativeSum(this double[][] matrix, int dimension)
        {
            int rows = matrix.Rows();
            int cols = matrix.Columns();
            if (dimension == 1)
                return CumulativeSum(matrix, dimension, Jagged.Zeros<double>(rows, cols));
            return CumulativeSum(matrix, dimension, Jagged.Zeros<double>(cols, rows));
        }

        /// <summary>
        ///   Matrix cumulative sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose cumulative sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the cumulative will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static double[][] CumulativeSum(this double[][] matrix, int dimension, double[][] result)
        {
            int rows = matrix.Rows();
            int cols = matrix.Columns();

            if (dimension == 1)
            {
                matrix.GetRow(0, result: result[0]);
                for (var i = 1; i < rows; i++)
                    for (var j = 0; j < cols; j++)
                        result[i][j] = (double)(result[i - 1][j] + matrix[i][j]);
            }
            else if (dimension == 0)
            {
                matrix.GetColumn(0, result: result[0]);
                for (var i = 1; i < cols; i++)
                    for (var j = 0; j < rows; j++)
                        result[i][j] = (double)(result[i - 1][j] + matrix[j][i]);
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix cumulative sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose cumulative sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the cumulative will be
        ///   calculated.</param>
        ///
        public static double[,] CumulativeSum(this double[,] matrix, int dimension)
        {
            int rows = matrix.Rows();
            int cols = matrix.Columns();
            if (dimension == 1)
                return CumulativeSum(matrix, dimension, Matrix.Zeros<double>(rows, cols));
            return CumulativeSum(matrix, dimension, Matrix.Zeros<double>(cols, rows));
        }

        /// <summary>
        ///   Matrix cumulative sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose cumulative sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the cumulative will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static double[,] CumulativeSum(this double[,] matrix, int dimension, double[,] result)
        {
            int rows = matrix.Rows();
            int cols = matrix.Columns();

            if (dimension == 1)
            {
                result.SetColumn(0, matrix.GetRow(0));
                for (var i = 1; i < rows; i++)
                    for (var j = 0; j < cols; j++)
                        result[i, j] = (double)(result[i - 1, j] + matrix[i, j]);
            }
            else if (dimension == 0)
            {
                result.SetColumn(0, matrix.GetColumn(0));
                for (var i = 1; i < cols; i++)
                    for (var j = 0; j < rows; j++)
                        result[i, j] = (double)(result[i - 1, j] + matrix[j, i]);
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        ///
        public static int[] Sum(this bool[][] matrix, int dimension)
        {
            var result = new int[Matrix.GetLength(matrix, dimension)];
            return Sum(matrix, dimension, result);
        }

        /// <summary>
        ///   Matrix sum.
        /// </summary>
        ///
        /// <param name="matrix">A matrix whose sum will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be
        ///   calculated.</param>
        /// <param name="result">A location where the result of this operation will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public static int[] Sum(this bool[][] matrix, int dimension, int[] result)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (dimension == 0)
            {
                for (int j = 0; j < cols; j++)
                {
                    int s = 0;
                    for (int i = 0; i < rows; i++)
                        s = (int)(s + (matrix[i][j] ? 1 : 0));
                    result[j] = s;
                }
            }
            else if (dimension == 1)
            {
                for (int j = 0; j < rows; j++)
                {
                    int s = 0;
                    for (int i = 0; i < cols; i++)
                        s = (int)(s + (matrix[j][i] ? 1 : 0));
                    result[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return result;
        }

        /// <summary>
        ///   Vector sum.
        /// </summary>
        ///
        /// <param name="vector">A vector whose sum will be calculated.</param>
        ///
        public static int Sum(this int[] vector)
        {
            int sum = 0;
            for (int i = 0; i < vector.Length; i++)
                sum = (int)(sum + (int)vector[i]);
            return sum;
        }
    }
}