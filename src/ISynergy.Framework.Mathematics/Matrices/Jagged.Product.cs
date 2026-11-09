using ISynergy.Framework.Mathematics.Abstractions;

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
        return Outer(a, b, new Jagged<int>(Jagged.Zeros<int>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<double>(Jagged.Zeros<double>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<float>(Jagged.Zeros<float>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<double>(Jagged.Zeros<double>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<double>(Jagged.Zeros<double>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<double>(Jagged.Zeros<double>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<float>(Jagged.Zeros<float>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<double>(Jagged.Zeros<double>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<float>(Jagged.Zeros<float>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<long>(Jagged.Zeros<long>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<double>(Jagged.Zeros<double>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<int>(Jagged.Zeros<int>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<float>(Jagged.Zeros<float>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<decimal>(Jagged.Zeros<decimal>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<double>(Jagged.Zeros<double>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<int>(Jagged.Zeros<int>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<float>(Jagged.Zeros<float>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<byte>(Jagged.Zeros<byte>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<double>(Jagged.Zeros<double>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<int>(Jagged.Zeros<int>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<float>(Jagged.Zeros<float>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<short>(Jagged.Zeros<short>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<double>(Jagged.Zeros<double>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<int>(Jagged.Zeros<int>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<float>(Jagged.Zeros<float>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<sbyte>(Jagged.Zeros<sbyte>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<double>(Jagged.Zeros<double>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<int>(Jagged.Zeros<int>(a.Length, b.Length)));
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
        return Outer(a, b, new Jagged<float>(Jagged.Zeros<float>(a.Length, b.Length)));
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
    public static int[][] Outer(int[] a, int[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static double[][] Outer(int[] a, int[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(int[] a, double[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * (int)b[j];
        return resultArray;
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
    public static double[][] Outer(int[] a, double[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(int[] a, float[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static float[][] Outer(int[] a, float[] b, Jagged<float> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (float)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static double[][] Outer(int[] a, float[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static double[][] Outer(double[] a, double[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(double[] a, double[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)a[i] * (int)b[j];
        return resultArray;
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
    public static double[][] Outer(double[] a, int[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(double[] a, int[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)a[i] * b[j];
        return resultArray;
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
    public static double[][] Outer(double[] a, float[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static float[][] Outer(double[] a, float[] b, Jagged<float> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (float)(a[i] * b[j]);
        return resultArray;
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
    public static int[][] Outer(double[] a, float[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)(a[i] * b[j]);
        return resultArray;
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
    public static float[][] Outer(float[] a, float[] b, Jagged<float> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (float)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static double[][] Outer(float[] a, float[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(float[] a, float[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static float[][] Outer(float[] a, double[] b, Jagged<float> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (float)(a[i] * b[j]);
        return resultArray;
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
    public static double[][] Outer(float[] a, double[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(float[] a, double[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)(a[i] * b[j]);
        return resultArray;
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
    public static float[][] Outer(float[] a, int[] b, Jagged<float> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (float)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static int[][] Outer(float[] a, int[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static double[][] Outer(float[] a, int[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static long[][] Outer(long[] a, long[] b, Jagged<long> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static double[][] Outer(long[] a, long[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(long[] a, long[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)a[i] * (int)b[j];
        return resultArray;
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
    public static long[][] Outer(long[] a, double[] b, Jagged<long> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * (long)b[j];
        return resultArray;
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
    public static double[][] Outer(long[] a, double[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(long[] a, double[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)a[i] * (int)b[j];
        return resultArray;
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
    public static long[][] Outer(long[] a, int[] b, Jagged<long> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(long[] a, int[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)a[i] * b[j];
        return resultArray;
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
    public static double[][] Outer(long[] a, int[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static long[][] Outer(long[] a, float[] b, Jagged<long> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (long)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static float[][] Outer(long[] a, float[] b, Jagged<float> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (float)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static double[][] Outer(long[] a, float[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(long[] a, float[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static decimal[][] Outer(decimal[] a, decimal[] b, Jagged<decimal> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static double[][] Outer(decimal[] a, decimal[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)(a[i] * b[j]);
        return resultArray;
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
    public static int[][] Outer(decimal[] a, decimal[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)a[i] * (int)b[j];
        return resultArray;
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
    public static decimal[][] Outer(decimal[] a, double[] b, Jagged<decimal> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * (decimal)b[j];
        return resultArray;
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
    public static double[][] Outer(decimal[] a, double[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(decimal[] a, double[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)a[i] * (int)b[j];
        return resultArray;
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
    public static decimal[][] Outer(decimal[] a, int[] b, Jagged<decimal> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(decimal[] a, int[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)a[i] * b[j];
        return resultArray;
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
    public static double[][] Outer(decimal[] a, int[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static decimal[][] Outer(decimal[] a, float[] b, Jagged<decimal> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (decimal)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static float[][] Outer(decimal[] a, float[] b, Jagged<float> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (float)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static double[][] Outer(decimal[] a, float[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(decimal[] a, float[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static byte[][] Outer(byte[] a, byte[] b, Jagged<byte> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (byte)(a[i] * b[j]);
        return resultArray;
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
    public static double[][] Outer(byte[] a, byte[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(byte[] a, byte[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static byte[][] Outer(byte[] a, double[] b, Jagged<byte> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (byte)(a[i] * (byte)b[j]);
        return resultArray;
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
    public static double[][] Outer(byte[] a, double[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(byte[] a, double[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * (int)b[j];
        return resultArray;
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
    public static byte[][] Outer(byte[] a, int[] b, Jagged<byte> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (byte)(a[i] * (byte)b[j]);
        return resultArray;
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
    public static int[][] Outer(byte[] a, int[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static double[][] Outer(byte[] a, int[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static byte[][] Outer(byte[] a, float[] b, Jagged<byte> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (byte)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static float[][] Outer(byte[] a, float[] b, Jagged<float> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (float)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static double[][] Outer(byte[] a, float[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(byte[] a, float[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static short[][] Outer(short[] a, short[] b, Jagged<short> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (short)(a[i] * b[j]);
        return resultArray;
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
    public static double[][] Outer(short[] a, short[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(short[] a, short[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static short[][] Outer(short[] a, double[] b, Jagged<short> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (short)(a[i] * (short)b[j]);
        return resultArray;
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
    public static double[][] Outer(short[] a, double[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(short[] a, double[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * (int)b[j];
        return resultArray;
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
    public static short[][] Outer(short[] a, int[] b, Jagged<short> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (short)(a[i] * (short)b[j]);
        return resultArray;
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
    public static int[][] Outer(short[] a, int[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static double[][] Outer(short[] a, int[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static short[][] Outer(short[] a, float[] b, Jagged<short> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (short)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static float[][] Outer(short[] a, float[] b, Jagged<float> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (float)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static double[][] Outer(short[] a, float[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(short[] a, float[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static sbyte[][] Outer(sbyte[] a, sbyte[] b, Jagged<sbyte> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (sbyte)(a[i] * b[j]);
        return resultArray;
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
    public static double[][] Outer(sbyte[] a, sbyte[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(sbyte[] a, sbyte[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static sbyte[][] Outer(sbyte[] a, double[] b, Jagged<sbyte> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (sbyte)(a[i] * (sbyte)b[j]);
        return resultArray;
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
    public static double[][] Outer(sbyte[] a, double[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(sbyte[] a, double[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * (int)b[j];
        return resultArray;
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
    public static sbyte[][] Outer(sbyte[] a, int[] b, Jagged<sbyte> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (sbyte)(a[i] * (sbyte)b[j]);
        return resultArray;
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
    public static int[][] Outer(sbyte[] a, int[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = a[i] * b[j];
        return resultArray;
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
    public static double[][] Outer(sbyte[] a, int[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static sbyte[][] Outer(sbyte[] a, float[] b, Jagged<sbyte> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (sbyte)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static float[][] Outer(sbyte[] a, float[] b, Jagged<float> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (float)((double)a[i] * (double)b[j]);
        return resultArray;
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
    public static double[][] Outer(sbyte[] a, float[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (double)a[i] * b[j];
        return resultArray;
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
    public static int[][] Outer(sbyte[] a, float[] b, Jagged<int> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (int)((double)a[i] * (double)b[j]);
        return resultArray;
    }

}