namespace ISynergy.Framework.Mathematics;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ISynergy.Framework.Mathematics.Distances;
using ISynergy.Framework.Mathematics.Decompositions;
using ISynergy.Framework.Mathematics.Common;

/// <summary>
///   Static class Distance. Defines a set of methods defining distance measures.
/// </summary>
/// 
public static partial class Norm
{
    /// <summary>
    ///   Gets the square root of the sum of squares for all elements in a matrix.
    /// </summary>
    /// 
    public static float Frobenius(this float[,] a)
    {
        return Euclidean(a);
    }

    /// <summary>
    ///   Gets the square root of the sum of squares for all elements in a matrix.
    /// </summary>
    /// 
    public static float Frobenius(this float[][] a)
    {
        return Euclidean(a);
    }

    /// <summary>
    ///   Gets the Squared Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static float SquareEuclidean(this float[,] a)
    {
        float sum = 0;
        foreach (var v in a)
            sum += v * v;
        return sum;
    }

    /// <summary>
    ///   Gets the Squared Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static float SquareEuclidean(this float[][] a)
    {
        float sum = 0;
        
        for (var j = 0; j < a.Length; j++)
        {
            for (var i = 0; i < a[j].Length; i++)
            {
                var v = a[j][i];
                sum += v * v;
            }
        }

        return sum;
    }

    /// <summary>
    ///   Gets the Squared Euclidean norm for a vector.
    /// </summary>
    /// 
    public static float SquareEuclidean(this float[] a)
    {
        float sum = 0;
        
        for (var j = 0; j < a.Length; j++)
        {
            var v = a[j];
            sum += v * v;
        }

        return sum;
    }

		/// <summary>
    ///   Gets the Squared Euclidean norm for a vector.
    /// </summary>
    /// 
    public static float SquareEuclidean(this Sparse<float> a)
    {
        float sum = 0;
        
        for (var j = 0; j < a.Length; j++)
        {
            var v = a.Values[j];
            sum += v * v;
        }

        return sum;
    }

    /// <summary>
    ///   Gets the Squared Euclidean norm vector for a matrix.
    /// </summary>
    /// 
    public static float[] SquareEuclidean(this float[,] a, int dimension)
    {
        int rows = a.Rows();
        int cols = a.Columns();

        float[] norm;

        if (dimension == 0)
        {
            norm = new float[cols];
            for (var j = 0; j < norm.Length; j++)
            {
                float sum = 0;
                for (var i = 0; i < rows; i++)
                {
                    var v = a[i, j];
                    sum += v * v;
                }
                norm[j] = sum;
            }
        }
        else
        {
            norm = new float[rows];
            for (var i = 0; i < norm.Length; i++)
            {
                float sum = 0;
                for (var j = 0; j < cols; j++)
                {
                    var v = a[i, j];
                    sum += v * v;
                }
                norm[i] = sum;
            }
        }

        return norm;
    }

    /// <summary>
    ///   Gets the Squared Euclidean norm vector for a matrix.
    /// </summary>
    /// 
    public static float[] SquareEuclidean(this float[][] a, int dimension)
    {
        int rows = a.Rows();
        int cols = a.Columns();

        float[] norm;

        if (dimension == 0)
        {
            norm = new float[cols];
            for (var j = 0; j < norm.Length; j++)
            {
                float sum = 0;
                for (var i = 0; i < rows; i++)
                {
                    var v = a[i][j];
                    sum += v * v;
                }
                norm[j] = sum;
            }
        }
        else
        {
            norm = new float[rows];
            for (var i = 0; i < norm.Length; i++)
            {
                float sum = 0;
                for (var j = 0; j < a[i].Length; j++)
                {
                    var v = a[i][j];
                    sum += v * v;
                }
                norm[i] = sum;
            }
        }

        return norm;
    }

		/// <summary>
    ///   Gets the Squared Euclidean norm vector for a matrix.
    /// </summary>
    /// 
    public static float[] SquareEuclidean(this Sparse<float>[] a, int dimension)
    {
        int rows = a.Rows();
        int cols = a.Columns();

        float[] norm;

        if (dimension == 0)
        {
            norm = new float[cols];
            for (var j = 0; j < norm.Length; j++)
            {
                float sum = 0;
                for (var i = 0; i < rows; i++)
                {
                    var v = a[i][j];
                    sum += v * v;
                }
                norm[j] = sum;
            }
        }
        else
        {
            norm = new float[rows];
            for (var i = 0; i < norm.Length; i++)
            {
                float sum = 0;
                for (var j = 0; j < a[i].Values.Length; j++)
                {
                    var v = a[i].Values[j];
                    sum += v * v;
                }
                norm[i] = sum;
            }
        }

        return norm;
    }

    /// <summary>
    ///   Gets the Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static float Euclidean(this float[,] a)
    {
        return (float)Math.Sqrt(SquareEuclidean(a));
    }

    /// <summary>
    ///   Gets the Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static float Euclidean(this float[][] a)
    {
        return (float)Math.Sqrt(SquareEuclidean(a));
    }

		/// <summary>
    ///   Gets the Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static float Euclidean(this Sparse<float> a)
    {
        return (float)Math.Sqrt(SquareEuclidean(a));
    }

    /// <summary>
    ///   Gets the Euclidean norm for a vector.
    /// </summary>
    /// 
    public static float Euclidean(this float[] a)
    {
        return (float)Math.Sqrt(SquareEuclidean(a));
    }

    /// <summary>
    ///   Gets the Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static float[] Euclidean(this float[,] a, int dimension)
    {
        var norm = Norm.SquareEuclidean(a, dimension);
        for (var i = 0; i < norm.Length; i++)
            norm[i] = (float)System.Math.Sqrt(norm[i]);
        return norm;
    }

    /// <summary>
    ///   Gets the Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static float[] Euclidean(this float[][] a, int dimension)
    {
        var norm = Norm.SquareEuclidean(a, dimension);
        for (var i = 0; i < norm.Length; i++)
            norm[i] = (float)System.Math.Sqrt(norm[i]);
        return norm;
    }

		/// <summary>
    ///   Gets the Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static float[] Euclidean(this Sparse<float>[] a, int dimension)
    {
        var norm = Norm.SquareEuclidean(a, dimension);
        for (var i = 0; i < norm.Length; i++)
            norm[i] = (float)System.Math.Sqrt(norm[i]);
        return norm;
    }
    /// <summary>
    ///   Gets the square root of the sum of squares for all elements in a matrix.
    /// </summary>
    /// 
    public static double Frobenius(this double[,] a)
    {
        return Euclidean(a);
    }

    /// <summary>
    ///   Gets the square root of the sum of squares for all elements in a matrix.
    /// </summary>
    /// 
    public static double Frobenius(this double[][] a)
    {
        return Euclidean(a);
    }

    /// <summary>
    ///   Gets the Squared Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static double SquareEuclidean(this double[,] a)
    {
        double sum = 0;
        foreach (var v in a)
            sum += v * v;
        return sum;
    }

    /// <summary>
    ///   Gets the Squared Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static double SquareEuclidean(this double[][] a)
    {
        double sum = 0;
        
        for (var j = 0; j < a.Length; j++)
        {
            for (var i = 0; i < a[j].Length; i++)
            {
                var v = a[j][i];
                sum += v * v;
            }
        }

        return sum;
    }

    /// <summary>
    ///   Gets the Squared Euclidean norm for a vector.
    /// </summary>
    /// 
    public static double SquareEuclidean(this double[] a)
    {
        double sum = 0;
        
        for (var j = 0; j < a.Length; j++)
        {
            var v = a[j];
            sum += v * v;
        }

        return sum;
    }

		/// <summary>
    ///   Gets the Squared Euclidean norm for a vector.
    /// </summary>
    /// 
    public static double SquareEuclidean(this Sparse<double> a)
    {
        double sum = 0;
        
        for (var j = 0; j < a.Length; j++)
        {
            var v = a.Values[j];
            sum += v * v;
        }

        return sum;
    }

    /// <summary>
    ///   Gets the Squared Euclidean norm vector for a matrix.
    /// </summary>
    /// 
    public static double[] SquareEuclidean(this double[,] a, int dimension)
    {
        int rows = a.Rows();
        int cols = a.Columns();

        double[] norm;

        if (dimension == 0)
        {
            norm = new double[cols];
            for (var j = 0; j < norm.Length; j++)
            {
                double sum = 0;
                for (var i = 0; i < rows; i++)
                {
                    var v = a[i, j];
                    sum += v * v;
                }
                norm[j] = sum;
            }
        }
        else
        {
            norm = new double[rows];
            for (var i = 0; i < norm.Length; i++)
            {
                double sum = 0;
                for (var j = 0; j < cols; j++)
                {
                    var v = a[i, j];
                    sum += v * v;
                }
                norm[i] = sum;
            }
        }

        return norm;
    }

    /// <summary>
    ///   Gets the Squared Euclidean norm vector for a matrix.
    /// </summary>
    /// 
    public static double[] SquareEuclidean(this double[][] a, int dimension)
    {
        int rows = a.Rows();
        int cols = a.Columns();

        double[] norm;

        if (dimension == 0)
        {
            norm = new double[cols];
            for (var j = 0; j < norm.Length; j++)
            {
                double sum = 0;
                for (var i = 0; i < rows; i++)
                {
                    var v = a[i][j];
                    sum += v * v;
                }
                norm[j] = sum;
            }
        }
        else
        {
            norm = new double[rows];
            for (var i = 0; i < norm.Length; i++)
            {
                double sum = 0;
                for (var j = 0; j < a[i].Length; j++)
                {
                    var v = a[i][j];
                    sum += v * v;
                }
                norm[i] = sum;
            }
        }

        return norm;
    }

		/// <summary>
    ///   Gets the Squared Euclidean norm vector for a matrix.
    /// </summary>
    /// 
    public static double[] SquareEuclidean(this Sparse<double>[] a, int dimension)
    {
        int rows = a.Rows();
        int cols = a.Columns();

        double[] norm;

        if (dimension == 0)
        {
            norm = new double[cols];
            for (var j = 0; j < norm.Length; j++)
            {
                double sum = 0;
                for (var i = 0; i < rows; i++)
                {
                    var v = a[i][j];
                    sum += v * v;
                }
                norm[j] = sum;
            }
        }
        else
        {
            norm = new double[rows];
            for (var i = 0; i < norm.Length; i++)
            {
                double sum = 0;
                for (var j = 0; j < a[i].Values.Length; j++)
                {
                    var v = a[i].Values[j];
                    sum += v * v;
                }
                norm[i] = sum;
            }
        }

        return norm;
    }

    /// <summary>
    ///   Gets the Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static double Euclidean(this double[,] a)
    {
        return (double)Math.Sqrt(SquareEuclidean(a));
    }

    /// <summary>
    ///   Gets the Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static double Euclidean(this double[][] a)
    {
        return (double)Math.Sqrt(SquareEuclidean(a));
    }

		/// <summary>
    ///   Gets the Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static double Euclidean(this Sparse<double> a)
    {
        return (double)Math.Sqrt(SquareEuclidean(a));
    }

    /// <summary>
    ///   Gets the Euclidean norm for a vector.
    /// </summary>
    /// 
    public static double Euclidean(this double[] a)
    {
        return (double)Math.Sqrt(SquareEuclidean(a));
    }

    /// <summary>
    ///   Gets the Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static double[] Euclidean(this double[,] a, int dimension)
    {
        var norm = Norm.SquareEuclidean(a, dimension);
        for (var i = 0; i < norm.Length; i++)
            norm[i] = (double)System.Math.Sqrt(norm[i]);
        return norm;
    }

    /// <summary>
    ///   Gets the Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static double[] Euclidean(this double[][] a, int dimension)
    {
        var norm = Norm.SquareEuclidean(a, dimension);
        for (var i = 0; i < norm.Length; i++)
            norm[i] = (double)System.Math.Sqrt(norm[i]);
        return norm;
    }

		/// <summary>
    ///   Gets the Euclidean norm for a matrix.
    /// </summary>
    /// 
    public static double[] Euclidean(this Sparse<double>[] a, int dimension)
    {
        var norm = Norm.SquareEuclidean(a, dimension);
        for (var i = 0; i < norm.Length; i++)
            norm[i] = (double)System.Math.Sqrt(norm[i]);
        return norm;
    }
}
