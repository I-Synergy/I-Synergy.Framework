using ISynergy.Framework.Mathematics.Vectors;

namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Jagged
{
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(int[] a, int[] b)
    {
        return Outer(a, b, Jagged.Zeros<int>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(int[] a, double[] b)
    {
        return Outer(a, b, Jagged.Zeros<double>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(int[] a, float[] b)
    {
        return Outer(a, b, Jagged.Zeros<float>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(double[] a, double[] b)
    {
        return Outer(a, b, Jagged.Zeros<double>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(double[] a, int[] b)
    {
        return Outer(a, b, Jagged.Zeros<double>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(double[] a, float[] b)
    {
        return Outer(a, b, Jagged.Zeros<double>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(float[] a, float[] b)
    {
        return Outer(a, b, Jagged.Zeros<float>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(float[] a, double[] b)
    {
        return Outer(a, b, Jagged.Zeros<double>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(float[] a, int[] b)
    {
        return Outer(a, b, Jagged.Zeros<float>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static long[][] Outer(long[] a, long[] b)
    {
        return Outer(a, b, Jagged.Zeros<long>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(long[] a, double[] b)
    {
        return Outer(a, b, Jagged.Zeros<double>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(long[] a, int[] b)
    {
        return Outer(a, b, Jagged.Zeros<int>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(long[] a, float[] b)
    {
        return Outer(a, b, Jagged.Zeros<float>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static decimal[][] Outer(decimal[] a, decimal[] b)
    {
        return Outer(a, b, Jagged.Zeros<decimal>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(decimal[] a, double[] b)
    {
        return Outer(a, b, Jagged.Zeros<double>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(decimal[] a, int[] b)
    {
        return Outer(a, b, Jagged.Zeros<int>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(decimal[] a, float[] b)
    {
        return Outer(a, b, Jagged.Zeros<float>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static byte[][] Outer(byte[] a, byte[] b)
    {
        return Outer(a, b, Jagged.Zeros<byte>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(byte[] a, double[] b)
    {
        return Outer(a, b, Jagged.Zeros<double>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(byte[] a, int[] b)
    {
        return Outer(a, b, Jagged.Zeros<int>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(byte[] a, float[] b)
    {
        return Outer(a, b, Jagged.Zeros<float>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static short[][] Outer(short[] a, short[] b)
    {
        return Outer(a, b, Jagged.Zeros<short>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(short[] a, double[] b)
    {
        return Outer(a, b, Jagged.Zeros<double>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(short[] a, int[] b)
    {
        return Outer(a, b, Jagged.Zeros<int>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(short[] a, float[] b)
    {
        return Outer(a, b, Jagged.Zeros<float>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static sbyte[][] Outer(sbyte[] a, sbyte[] b)
    {
        return Outer(a, b, Jagged.Zeros<sbyte>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(sbyte[] a, double[] b)
    {
        return Outer(a, b, Jagged.Zeros<double>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(sbyte[] a, int[] b)
    {
        return Outer(a, b, Jagged.Zeros<int>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(sbyte[] a, float[] b)
    {
        return Outer(a, b, Jagged.Zeros<float>(a.Length, b.Length));
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(int[] a, int[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(int[] a, int[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(int[] a, double[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(int[] a, double[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(int[] a, float[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(int[] a, float[] b, float[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (float)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(int[] a, float[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(double[] a, double[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(double[] a, double[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(double[] a, int[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(double[] a, int[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(double[] a, float[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(double[] a, float[] b, float[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (float)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(double[] a, float[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(float[] a, float[] b, float[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (float)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(float[] a, float[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(float[] a, float[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(float[] a, double[] b, float[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (float)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(float[] a, double[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(float[] a, double[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(float[] a, int[] b, float[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (float)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(float[] a, int[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(float[] a, int[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static long[][] Outer(long[] a, long[] b, long[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (long)((long)a[i] * (long)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(long[] a, long[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(long[] a, long[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static long[][] Outer(long[] a, double[] b, long[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (long)((long)a[i] * (long)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(long[] a, double[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(long[] a, double[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static long[][] Outer(long[] a, int[] b, long[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (long)((long)a[i] * (long)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(long[] a, int[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(long[] a, int[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static long[][] Outer(long[] a, float[] b, long[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (long)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(long[] a, float[] b, float[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (float)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(long[] a, float[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(long[] a, float[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static decimal[][] Outer(decimal[] a, decimal[] b, decimal[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (decimal)((decimal)a[i] * (decimal)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(decimal[] a, decimal[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(decimal[] a, decimal[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static decimal[][] Outer(decimal[] a, double[] b, decimal[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (decimal)((decimal)a[i] * (decimal)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(decimal[] a, double[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(decimal[] a, double[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static decimal[][] Outer(decimal[] a, int[] b, decimal[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (decimal)((decimal)a[i] * (decimal)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(decimal[] a, int[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(decimal[] a, int[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static decimal[][] Outer(decimal[] a, float[] b, decimal[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (decimal)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(decimal[] a, float[] b, float[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (float)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(decimal[] a, float[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(decimal[] a, float[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static byte[][] Outer(byte[] a, byte[] b, byte[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (byte)((byte)a[i] * (byte)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(byte[] a, byte[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(byte[] a, byte[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static byte[][] Outer(byte[] a, double[] b, byte[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (byte)((byte)a[i] * (byte)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(byte[] a, double[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(byte[] a, double[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static byte[][] Outer(byte[] a, int[] b, byte[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (byte)((byte)a[i] * (byte)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(byte[] a, int[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(byte[] a, int[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static byte[][] Outer(byte[] a, float[] b, byte[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (byte)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(byte[] a, float[] b, float[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (float)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(byte[] a, float[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(byte[] a, float[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static short[][] Outer(short[] a, short[] b, short[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (short)((short)a[i] * (short)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(short[] a, short[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(short[] a, short[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static short[][] Outer(short[] a, double[] b, short[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (short)((short)a[i] * (short)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(short[] a, double[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(short[] a, double[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static short[][] Outer(short[] a, int[] b, short[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (short)((short)a[i] * (short)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(short[] a, int[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(short[] a, int[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static short[][] Outer(short[] a, float[] b, short[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (short)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(short[] a, float[] b, float[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (float)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(short[] a, float[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(short[] a, float[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static sbyte[][] Outer(sbyte[] a, sbyte[] b, sbyte[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (sbyte)((sbyte)a[i] * (sbyte)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(sbyte[] a, sbyte[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(sbyte[] a, sbyte[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static sbyte[][] Outer(sbyte[] a, double[] b, sbyte[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (sbyte)((sbyte)a[i] * (sbyte)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(sbyte[] a, double[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(sbyte[] a, double[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static sbyte[][] Outer(sbyte[] a, int[] b, sbyte[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (sbyte)((sbyte)a[i] * (sbyte)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(sbyte[] a, int[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((int)a[i] * (int)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(sbyte[] a, int[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static sbyte[][] Outer(sbyte[] a, float[] b, sbyte[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (sbyte)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static float[][] Outer(sbyte[] a, float[] b, float[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (float)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(sbyte[] a, float[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (double)((double)a[i] * (double)b[j]);
        return result;
    }
    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static int[][] Outer(sbyte[] a, float[] b, int[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (int)((double)a[i] * (double)b[j]);
        return result;
    }

}