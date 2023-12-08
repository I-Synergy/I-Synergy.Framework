namespace ISynergy.Framework.Mathematics;

using System;
using System.CodeDom.Compiler;
using ISynergy.Framework.Mathematics;
using System.Runtime.CompilerServices;
	using System.Numerics;

public static partial class Matrix
{
    /// <summary>
    ///   Converts a integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[] ToDouble(this int[] value)
    {
        return ToDouble(value, new double[value.Length]);
    }

    /// <summary>
    ///   Converts a integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,] ToDouble(this int[,] value)
    {
        return ToDouble(value, Matrix.CreateAs<int, double>(value));
    }

		/// <summary>
    ///   Converts a integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this int[,,] value)
    {
        return ToDouble(value, Matrix.CreateAs<int, double>(value));
    }

    /// <summary>
    ///   Converts a integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][] ToDouble(this int[][] value)
    {
        return ToDouble(value, Jagged.CreateAs<int, double>(value));
    }

    /// <summary>
    ///   Converts a integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this int[][][] value)
    {
        return ToDouble(value, Jagged.CreateAs<int, double>(value));
    }

    /// <summary>
    ///   Converts a integer array to a double-precision floating point array.
    /// </summary>
    /// 
    public static double[] ToDouble(this int[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Double)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional integer array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this int[,] value, double[,] result)
    {
					unsafe
			{
				fixed (int* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional integer array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this int[,,] value, double[,,] result)
    {
					unsafe
			{
				fixed (int* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional integer array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this int[,] value, double[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Double)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged integer array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this int[][] value, double[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Double)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged integer array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this int[][][] value, double[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Double)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged integer array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this int[][] value, double[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Double)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this int[] value)
    {
        return ToComplex(value, new Complex[value.Length]);
    }

    /// <summary>
    ///   Converts a integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this int[,] value)
    {
        return ToComplex(value, Matrix.CreateAs<int, Complex>(value));
    }

		/// <summary>
    ///   Converts a integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this int[,,] value)
    {
        return ToComplex(value, Matrix.CreateAs<int, Complex>(value));
    }

    /// <summary>
    ///   Converts a integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this int[][] value)
    {
        return ToComplex(value, Jagged.CreateAs<int, Complex>(value));
    }

    /// <summary>
    ///   Converts a integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this int[][][] value)
    {
        return ToComplex(value, Jagged.CreateAs<int, Complex>(value));
    }

    /// <summary>
    ///   Converts a integer array to a 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this int[] value, Complex[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Complex)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional integer array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this int[,] value, Complex[,] result)
    {
					unsafe
			{
				fixed (int* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional integer array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this int[,,] value, Complex[,,] result)
    {
					unsafe
			{
				fixed (int* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional integer array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this int[,] value, Complex[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Complex)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged integer array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this int[][] value, Complex[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Complex)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged integer array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this int[][][] value, Complex[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Complex)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged integer array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this int[][] value, Complex[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Complex)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a short integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[] ToDouble(this short[] value)
    {
        return ToDouble(value, new double[value.Length]);
    }

    /// <summary>
    ///   Converts a short integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,] ToDouble(this short[,] value)
    {
        return ToDouble(value, Matrix.CreateAs<short, double>(value));
    }

		/// <summary>
    ///   Converts a short integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this short[,,] value)
    {
        return ToDouble(value, Matrix.CreateAs<short, double>(value));
    }

    /// <summary>
    ///   Converts a short integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][] ToDouble(this short[][] value)
    {
        return ToDouble(value, Jagged.CreateAs<short, double>(value));
    }

    /// <summary>
    ///   Converts a short integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this short[][][] value)
    {
        return ToDouble(value, Jagged.CreateAs<short, double>(value));
    }

    /// <summary>
    ///   Converts a short integer array to a double-precision floating point array.
    /// </summary>
    /// 
    public static double[] ToDouble(this short[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Double)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional short integer array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this short[,] value, double[,] result)
    {
					unsafe
			{
				fixed (short* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional short integer array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this short[,,] value, double[,,] result)
    {
					unsafe
			{
				fixed (short* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional short integer array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this short[,] value, double[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Double)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged short integer array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this short[][] value, double[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Double)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged short integer array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this short[][][] value, double[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Double)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged short integer array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this short[][] value, double[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Double)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a short integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this short[] value)
    {
        return ToComplex(value, new Complex[value.Length]);
    }

    /// <summary>
    ///   Converts a short integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this short[,] value)
    {
        return ToComplex(value, Matrix.CreateAs<short, Complex>(value));
    }

		/// <summary>
    ///   Converts a short integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this short[,,] value)
    {
        return ToComplex(value, Matrix.CreateAs<short, Complex>(value));
    }

    /// <summary>
    ///   Converts a short integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this short[][] value)
    {
        return ToComplex(value, Jagged.CreateAs<short, Complex>(value));
    }

    /// <summary>
    ///   Converts a short integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this short[][][] value)
    {
        return ToComplex(value, Jagged.CreateAs<short, Complex>(value));
    }

    /// <summary>
    ///   Converts a short integer array to a 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this short[] value, Complex[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Complex)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional short integer array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this short[,] value, Complex[,] result)
    {
					unsafe
			{
				fixed (short* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional short integer array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this short[,,] value, Complex[,,] result)
    {
					unsafe
			{
				fixed (short* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional short integer array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this short[,] value, Complex[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Complex)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged short integer array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this short[][] value, Complex[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Complex)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged short integer array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this short[][][] value, Complex[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Complex)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged short integer array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this short[][] value, Complex[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Complex)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a single-precision floating point to a double-precision floating point.
    /// </summary>
    /// 
    public static double[] ToDouble(this float[] value)
    {
        return ToDouble(value, new double[value.Length]);
    }

    /// <summary>
    ///   Converts a single-precision floating point to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,] ToDouble(this float[,] value)
    {
        return ToDouble(value, Matrix.CreateAs<float, double>(value));
    }

		/// <summary>
    ///   Converts a single-precision floating point to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this float[,,] value)
    {
        return ToDouble(value, Matrix.CreateAs<float, double>(value));
    }

    /// <summary>
    ///   Converts a single-precision floating point to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][] ToDouble(this float[][] value)
    {
        return ToDouble(value, Jagged.CreateAs<float, double>(value));
    }

    /// <summary>
    ///   Converts a single-precision floating point to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this float[][][] value)
    {
        return ToDouble(value, Jagged.CreateAs<float, double>(value));
    }

    /// <summary>
    ///   Converts a single-precision floating point array to a double-precision floating point array.
    /// </summary>
    /// 
    public static double[] ToDouble(this float[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Double)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional single-precision floating point array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this float[,] value, double[,] result)
    {
					unsafe
			{
				fixed (float* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional single-precision floating point array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this float[,,] value, double[,,] result)
    {
					unsafe
			{
				fixed (float* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional single-precision floating point array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this float[,] value, double[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Double)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged single-precision floating point array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this float[][] value, double[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Double)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged single-precision floating point array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this float[][][] value, double[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Double)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged single-precision floating point array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this float[][] value, double[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Double)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a single-precision floating point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this float[] value)
    {
        return ToComplex(value, new Complex[value.Length]);
    }

    /// <summary>
    ///   Converts a single-precision floating point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this float[,] value)
    {
        return ToComplex(value, Matrix.CreateAs<float, Complex>(value));
    }

		/// <summary>
    ///   Converts a single-precision floating point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this float[,,] value)
    {
        return ToComplex(value, Matrix.CreateAs<float, Complex>(value));
    }

    /// <summary>
    ///   Converts a single-precision floating point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this float[][] value)
    {
        return ToComplex(value, Jagged.CreateAs<float, Complex>(value));
    }

    /// <summary>
    ///   Converts a single-precision floating point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this float[][][] value)
    {
        return ToComplex(value, Jagged.CreateAs<float, Complex>(value));
    }

    /// <summary>
    ///   Converts a single-precision floating point array to a 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this float[] value, Complex[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Complex)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional single-precision floating point array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this float[,] value, Complex[,] result)
    {
					unsafe
			{
				fixed (float* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional single-precision floating point array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this float[,,] value, Complex[,,] result)
    {
					unsafe
			{
				fixed (float* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional single-precision floating point array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this float[,] value, Complex[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Complex)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged single-precision floating point array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this float[][] value, Complex[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Complex)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged single-precision floating point array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this float[][][] value, Complex[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Complex)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged single-precision floating point array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this float[][] value, Complex[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Complex)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a double-precision floating point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this double[] value)
    {
        return ToComplex(value, new Complex[value.Length]);
    }

    /// <summary>
    ///   Converts a double-precision floating point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this double[,] value)
    {
        return ToComplex(value, Matrix.CreateAs<double, Complex>(value));
    }

		/// <summary>
    ///   Converts a double-precision floating point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this double[,,] value)
    {
        return ToComplex(value, Matrix.CreateAs<double, Complex>(value));
    }

    /// <summary>
    ///   Converts a double-precision floating point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this double[][] value)
    {
        return ToComplex(value, Jagged.CreateAs<double, Complex>(value));
    }

    /// <summary>
    ///   Converts a double-precision floating point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this double[][][] value)
    {
        return ToComplex(value, Jagged.CreateAs<double, Complex>(value));
    }

    /// <summary>
    ///   Converts a double-precision floating point array to a 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this double[] value, Complex[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Complex)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional double-precision floating point array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this double[,] value, Complex[,] result)
    {
					unsafe
			{
				fixed (double* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional double-precision floating point array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this double[,,] value, Complex[,,] result)
    {
					unsafe
			{
				fixed (double* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional double-precision floating point array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this double[,] value, Complex[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Complex)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged double-precision floating point array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this double[][] value, Complex[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Complex)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged double-precision floating point array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this double[][][] value, Complex[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Complex)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged double-precision floating point array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this double[][] value, Complex[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Complex)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a long integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[] ToDouble(this long[] value)
    {
        return ToDouble(value, new double[value.Length]);
    }

    /// <summary>
    ///   Converts a long integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,] ToDouble(this long[,] value)
    {
        return ToDouble(value, Matrix.CreateAs<long, double>(value));
    }

		/// <summary>
    ///   Converts a long integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this long[,,] value)
    {
        return ToDouble(value, Matrix.CreateAs<long, double>(value));
    }

    /// <summary>
    ///   Converts a long integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][] ToDouble(this long[][] value)
    {
        return ToDouble(value, Jagged.CreateAs<long, double>(value));
    }

    /// <summary>
    ///   Converts a long integer to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this long[][][] value)
    {
        return ToDouble(value, Jagged.CreateAs<long, double>(value));
    }

    /// <summary>
    ///   Converts a long integer array to a double-precision floating point array.
    /// </summary>
    /// 
    public static double[] ToDouble(this long[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Double)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional long integer array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this long[,] value, double[,] result)
    {
					unsafe
			{
				fixed (long* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional long integer array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this long[,,] value, double[,,] result)
    {
					unsafe
			{
				fixed (long* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional long integer array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this long[,] value, double[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Double)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged long integer array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this long[][] value, double[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Double)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged long integer array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this long[][][] value, double[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Double)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged long integer array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this long[][] value, double[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Double)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a long integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this long[] value)
    {
        return ToComplex(value, new Complex[value.Length]);
    }

    /// <summary>
    ///   Converts a long integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this long[,] value)
    {
        return ToComplex(value, Matrix.CreateAs<long, Complex>(value));
    }

		/// <summary>
    ///   Converts a long integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this long[,,] value)
    {
        return ToComplex(value, Matrix.CreateAs<long, Complex>(value));
    }

    /// <summary>
    ///   Converts a long integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this long[][] value)
    {
        return ToComplex(value, Jagged.CreateAs<long, Complex>(value));
    }

    /// <summary>
    ///   Converts a long integer to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this long[][][] value)
    {
        return ToComplex(value, Jagged.CreateAs<long, Complex>(value));
    }

    /// <summary>
    ///   Converts a long integer array to a 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this long[] value, Complex[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Complex)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional long integer array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this long[,] value, Complex[,] result)
    {
					unsafe
			{
				fixed (long* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional long integer array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this long[,,] value, Complex[,,] result)
    {
					unsafe
			{
				fixed (long* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional long integer array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this long[,] value, Complex[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Complex)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged long integer array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this long[][] value, Complex[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Complex)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged long integer array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this long[][][] value, Complex[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Complex)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged long integer array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this long[][] value, Complex[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Complex)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a 8-bit byte to a double-precision floating point.
    /// </summary>
    /// 
    public static double[] ToDouble(this byte[] value)
    {
        return ToDouble(value, new double[value.Length]);
    }

    /// <summary>
    ///   Converts a 8-bit byte to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,] ToDouble(this byte[,] value)
    {
        return ToDouble(value, Matrix.CreateAs<byte, double>(value));
    }

		/// <summary>
    ///   Converts a 8-bit byte to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this byte[,,] value)
    {
        return ToDouble(value, Matrix.CreateAs<byte, double>(value));
    }

    /// <summary>
    ///   Converts a 8-bit byte to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][] ToDouble(this byte[][] value)
    {
        return ToDouble(value, Jagged.CreateAs<byte, double>(value));
    }

    /// <summary>
    ///   Converts a 8-bit byte to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this byte[][][] value)
    {
        return ToDouble(value, Jagged.CreateAs<byte, double>(value));
    }

    /// <summary>
    ///   Converts a 8-bit byte array to a double-precision floating point array.
    /// </summary>
    /// 
    public static double[] ToDouble(this byte[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Double)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional 8-bit byte array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this byte[,] value, double[,] result)
    {
					unsafe
			{
				fixed (byte* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional 8-bit byte array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this byte[,,] value, double[,,] result)
    {
					unsafe
			{
				fixed (byte* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional 8-bit byte array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this byte[,] value, double[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Double)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged 8-bit byte array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this byte[][] value, double[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Double)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged 8-bit byte array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this byte[][][] value, double[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Double)value[i][j][k];
        return result;            
    }
    
    /// <summary>
    ///   Converts a 8-bit byte to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this byte[] value)
    {
        return ToComplex(value, new Complex[value.Length]);
    }

    /// <summary>
    ///   Converts a 8-bit byte to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this byte[,] value)
    {
        return ToComplex(value, Matrix.CreateAs<byte, Complex>(value));
    }

		/// <summary>
    ///   Converts a 8-bit byte to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this byte[,,] value)
    {
        return ToComplex(value, Matrix.CreateAs<byte, Complex>(value));
    }

    /// <summary>
    ///   Converts a 8-bit byte to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this byte[][] value)
    {
        return ToComplex(value, Jagged.CreateAs<byte, Complex>(value));
    }

    /// <summary>
    ///   Converts a 8-bit byte to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this byte[][][] value)
    {
        return ToComplex(value, Jagged.CreateAs<byte, Complex>(value));
    }

    /// <summary>
    ///   Converts a 8-bit byte array to a 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this byte[] value, Complex[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Complex)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional 8-bit byte array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this byte[,] value, Complex[,] result)
    {
					unsafe
			{
				fixed (byte* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional 8-bit byte array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this byte[,,] value, Complex[,,] result)
    {
					unsafe
			{
				fixed (byte* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional 8-bit byte array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this byte[,] value, Complex[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Complex)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged 8-bit byte array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this byte[][] value, Complex[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Complex)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged 8-bit byte array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this byte[][][] value, Complex[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Complex)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged 8-bit byte array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this byte[][] value, Complex[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Complex)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a signed 7-bit byte to a double-precision floating point.
    /// </summary>
    /// 
    public static double[] ToDouble(this sbyte[] value)
    {
        return ToDouble(value, new double[value.Length]);
    }

    /// <summary>
    ///   Converts a signed 7-bit byte to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,] ToDouble(this sbyte[,] value)
    {
        return ToDouble(value, Matrix.CreateAs<sbyte, double>(value));
    }

		/// <summary>
    ///   Converts a signed 7-bit byte to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this sbyte[,,] value)
    {
        return ToDouble(value, Matrix.CreateAs<sbyte, double>(value));
    }

    /// <summary>
    ///   Converts a signed 7-bit byte to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][] ToDouble(this sbyte[][] value)
    {
        return ToDouble(value, Jagged.CreateAs<sbyte, double>(value));
    }

    /// <summary>
    ///   Converts a signed 7-bit byte to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this sbyte[][][] value)
    {
        return ToDouble(value, Jagged.CreateAs<sbyte, double>(value));
    }

    /// <summary>
    ///   Converts a signed 7-bit byte array to a double-precision floating point array.
    /// </summary>
    /// 
    public static double[] ToDouble(this sbyte[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Double)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional signed 7-bit byte array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this sbyte[,] value, double[,] result)
    {
					unsafe
			{
				fixed (sbyte* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional signed 7-bit byte array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this sbyte[,,] value, double[,,] result)
    {
					unsafe
			{
				fixed (sbyte* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional signed 7-bit byte array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this sbyte[,] value, double[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Double)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged signed 7-bit byte array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this sbyte[][] value, double[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Double)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged signed 7-bit byte array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this sbyte[][][] value, double[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Double)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged signed 7-bit byte array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this sbyte[][] value, double[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Double)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a signed 7-bit byte to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this sbyte[] value)
    {
        return ToComplex(value, new Complex[value.Length]);
    }

    /// <summary>
    ///   Converts a signed 7-bit byte to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this sbyte[,] value)
    {
        return ToComplex(value, Matrix.CreateAs<sbyte, Complex>(value));
    }

		/// <summary>
    ///   Converts a signed 7-bit byte to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this sbyte[,,] value)
    {
        return ToComplex(value, Matrix.CreateAs<sbyte, Complex>(value));
    }

    /// <summary>
    ///   Converts a signed 7-bit byte to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this sbyte[][] value)
    {
        return ToComplex(value, Jagged.CreateAs<sbyte, Complex>(value));
    }

    /// <summary>
    ///   Converts a signed 7-bit byte to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this sbyte[][][] value)
    {
        return ToComplex(value, Jagged.CreateAs<sbyte, Complex>(value));
    }

    /// <summary>
    ///   Converts a signed 7-bit byte array to a 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this sbyte[] value, Complex[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Complex)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional signed 7-bit byte array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this sbyte[,] value, Complex[,] result)
    {
					unsafe
			{
				fixed (sbyte* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional signed 7-bit byte array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this sbyte[,,] value, Complex[,,] result)
    {
					unsafe
			{
				fixed (sbyte* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional signed 7-bit byte array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this sbyte[,] value, Complex[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Complex)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged signed 7-bit byte array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this sbyte[][] value, Complex[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Complex)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged signed 7-bit byte array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this sbyte[][][] value, Complex[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Complex)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged signed 7-bit byte array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this sbyte[][] value, Complex[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Complex)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a decimal fixed-point to a double-precision floating point.
    /// </summary>
    /// 
    public static double[] ToDouble(this decimal[] value)
    {
        return ToDouble(value, new double[value.Length]);
    }

    /// <summary>
    ///   Converts a decimal fixed-point to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,] ToDouble(this decimal[,] value)
    {
        return ToDouble(value, Matrix.CreateAs<decimal, double>(value));
    }

		/// <summary>
    ///   Converts a decimal fixed-point to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this decimal[,,] value)
    {
        return ToDouble(value, Matrix.CreateAs<decimal, double>(value));
    }

    /// <summary>
    ///   Converts a decimal fixed-point to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][] ToDouble(this decimal[][] value)
    {
        return ToDouble(value, Jagged.CreateAs<decimal, double>(value));
    }

    /// <summary>
    ///   Converts a decimal fixed-point to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this decimal[][][] value)
    {
        return ToDouble(value, Jagged.CreateAs<decimal, double>(value));
    }

    /// <summary>
    ///   Converts a decimal fixed-point array to a double-precision floating point array.
    /// </summary>
    /// 
    public static double[] ToDouble(this decimal[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Double)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional decimal fixed-point array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this decimal[,] value, double[,] result)
    {
					unsafe
			{
				fixed (decimal* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional decimal fixed-point array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this decimal[,,] value, double[,,] result)
    {
					unsafe
			{
				fixed (decimal* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional decimal fixed-point array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this decimal[,] value, double[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Double)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged decimal fixed-point array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this decimal[][] value, double[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Double)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged decimal fixed-point array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this decimal[][][] value, double[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Double)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged decimal fixed-point array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this decimal[][] value, double[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Double)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a decimal fixed-point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this decimal[] value)
    {
        return ToComplex(value, new Complex[value.Length]);
    }

    /// <summary>
    ///   Converts a decimal fixed-point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this decimal[,] value)
    {
        return ToComplex(value, Matrix.CreateAs<decimal, Complex>(value));
    }

		/// <summary>
    ///   Converts a decimal fixed-point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this decimal[,,] value)
    {
        return ToComplex(value, Matrix.CreateAs<decimal, Complex>(value));
    }

    /// <summary>
    ///   Converts a decimal fixed-point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this decimal[][] value)
    {
        return ToComplex(value, Jagged.CreateAs<decimal, Complex>(value));
    }

    /// <summary>
    ///   Converts a decimal fixed-point to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this decimal[][][] value)
    {
        return ToComplex(value, Jagged.CreateAs<decimal, Complex>(value));
    }

    /// <summary>
    ///   Converts a decimal fixed-point array to a 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this decimal[] value, Complex[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Complex)value[i];
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional decimal fixed-point array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this decimal[,] value, Complex[,] result)
    {
					unsafe
			{
				fixed (decimal* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional decimal fixed-point array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this decimal[,,] value, Complex[,,] result)
    {
					unsafe
			{
				fixed (decimal* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Complex)src[i];
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional decimal fixed-point array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this decimal[,] value, Complex[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Complex)value[i, j];
        return result;
    }

    /// <summary>
    ///   Converts a jagged decimal fixed-point array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this decimal[][] value, Complex[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Complex)value[i][j];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged decimal fixed-point array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this decimal[][][] value, Complex[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Complex)value[i][j][k];
        return result;            
    }

    /// <summary>
    ///   Converts a jagged decimal fixed-point array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this decimal[][] value, Complex[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Complex)value[i][j];
        return result;            
    }
    
    /// <summary>
    ///   Converts a boolean to a double-precision floating point.
    /// </summary>
    /// 
    public static double[] ToDouble(this bool[] value)
    {
        return ToDouble(value, new double[value.Length]);
    }

    /// <summary>
    ///   Converts a boolean to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,] ToDouble(this bool[,] value)
    {
        return ToDouble(value, Matrix.CreateAs<bool, double>(value));
    }

		/// <summary>
    ///   Converts a boolean to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this bool[,,] value)
    {
        return ToDouble(value, Matrix.CreateAs<bool, double>(value));
    }

    /// <summary>
    ///   Converts a boolean to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][] ToDouble(this bool[][] value)
    {
        return ToDouble(value, Jagged.CreateAs<bool, double>(value));
    }

    /// <summary>
    ///   Converts a boolean to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this bool[][][] value)
    {
        return ToDouble(value, Jagged.CreateAs<bool, double>(value));
    }

    /// <summary>
    ///   Converts a boolean array to a double-precision floating point array.
    /// </summary>
    /// 
    public static double[] ToDouble(this bool[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = value[i] ? (Double)1 : (Double)0;
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional boolean array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this bool[,] value, double[,] result)
    {
					unsafe
			{
				fixed (bool* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = src[i] ? (Double)1 : (Double)0;
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional boolean array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this bool[,,] value, double[,,] result)
    {
					unsafe
			{
				fixed (bool* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = src[i] ? (Double)1 : (Double)0;
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional boolean array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this bool[,] value, double[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = value[i, j] ? (Double)1 : (Double)0;
        return result;
    }

    /// <summary>
    ///   Converts a jagged boolean array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this bool[][] value, double[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = value[i][j] ? (Double)1 : (Double)0;
        return result;            
    }

    /// <summary>
    ///   Converts a jagged boolean array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this bool[][][] value, double[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = value[i][j][k] ? (Double)1 : (Double)0;
        return result;            
    }

    /// <summary>
    ///   Converts a jagged boolean array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this bool[][] value, double[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = value[i][j] ? (Double)1 : (Double)0;
        return result;            
    }
    
    /// <summary>
    ///   Converts a boolean to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this bool[] value)
    {
        return ToComplex(value, new Complex[value.Length]);
    }

    /// <summary>
    ///   Converts a boolean to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this bool[,] value)
    {
        return ToComplex(value, Matrix.CreateAs<bool, Complex>(value));
    }

		/// <summary>
    ///   Converts a boolean to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this bool[,,] value)
    {
        return ToComplex(value, Matrix.CreateAs<bool, Complex>(value));
    }

    /// <summary>
    ///   Converts a boolean to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this bool[][] value)
    {
        return ToComplex(value, Jagged.CreateAs<bool, Complex>(value));
    }

    /// <summary>
    ///   Converts a boolean to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this bool[][][] value)
    {
        return ToComplex(value, Jagged.CreateAs<bool, Complex>(value));
    }

    /// <summary>
    ///   Converts a boolean array to a 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this bool[] value, Complex[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = value[i] ? (Complex)1 : (Complex)0;
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional boolean array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this bool[,] value, Complex[,] result)
    {
					unsafe
			{
				fixed (bool* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = src[i] ? (Complex)1 : (Complex)0;
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional boolean array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this bool[,,] value, Complex[,,] result)
    {
					unsafe
			{
				fixed (bool* src = value)
				fixed (Complex* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = src[i] ? (Complex)1 : (Complex)0;
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional boolean array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this bool[,] value, Complex[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = value[i, j] ? (Complex)1 : (Complex)0;
        return result;
    }

    /// <summary>
    ///   Converts a jagged boolean array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this bool[][] value, Complex[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = value[i][j] ? (Complex)1 : (Complex)0;
        return result;            
    }

    /// <summary>
    ///   Converts a jagged boolean array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this bool[][][] value, Complex[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = value[i][j][k] ? (Complex)1 : (Complex)0;
        return result;            
    }

    /// <summary>
    ///   Converts a jagged boolean array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this bool[][] value, Complex[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = value[i][j] ? (Complex)1 : (Complex)0;
        return result;            
    }
   
    /// <summary>
    ///   Converts a object to a double-precision floating point.
    /// </summary>
    /// 
    public static double[] ToDouble(this object[] value)
    {
        return ToDouble(value, new double[value.Length]);
    }

    /// <summary>
    ///   Converts a object to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,] ToDouble(this object[,] value)
    {
        return ToDouble(value, Matrix.CreateAs<object, double>(value));
    }

		/// <summary>
    ///   Converts a object to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this object[,,] value)
    {
        return ToDouble(value, Matrix.CreateAs<object, double>(value));
    }

    /// <summary>
    ///   Converts a object to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][] ToDouble(this object[][] value)
    {
        return ToDouble(value, Jagged.CreateAs<object, double>(value));
    }

    /// <summary>
    ///   Converts a object to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this object[][][] value)
    {
        return ToDouble(value, Jagged.CreateAs<object, double>(value));
    }

    /// <summary>
    ///   Converts a object array to a double-precision floating point array.
    /// </summary>
    /// 
    public static double[] ToDouble(this object[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Double)System.Convert.ChangeType(value[i], typeof(Double));
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional object array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this object[,] value, double[,] result)
    {
					int r = value.GetLength(0);
			int c = value.GetLength(1);
			for (var i = 0; i < r; i++)
				for (var j = 0; j < c; j++)
					result[i, j] = (Double)System.Convert.ChangeType(value[i, j], typeof(Double));
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional object array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this object[,,] value, double[,,] result)
    {
					int r = value.GetLength(0);
			int c = value.GetLength(1);
			int d = value.GetLength(2);
			for (var i = 0; i < r; i++)
				for (var j = 0; j < c; j++)
					for (var k = 0; k < d; k++)
					result[i, j, k] = (Double)System.Convert.ChangeType(value[i, j, k], typeof(Double));
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional object array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this object[,] value, double[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Double)System.Convert.ChangeType(value[i, j], typeof(Double));
        return result;
    }

    /// <summary>
    ///   Converts a jagged object array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this object[][] value, double[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Double)System.Convert.ChangeType(value[i][j], typeof(Double));
        return result;            
    }

    /// <summary>
    ///   Converts a jagged object array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this object[][][] value, double[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Double)System.Convert.ChangeType(value[i][j][k], typeof(Double));
        return result;            
    }

    /// <summary>
    ///   Converts a jagged object array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this object[][] value, double[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Double)System.Convert.ChangeType(value[i][j], typeof(Double));
        return result;            
    }
    
    /// <summary>
    ///   Converts a object to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this object[] value)
    {
        return ToComplex(value, new Complex[value.Length]);
    }

    /// <summary>
    ///   Converts a object to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this object[,] value)
    {
        return ToComplex(value, Matrix.CreateAs<object, Complex>(value));
    }

		/// <summary>
    ///   Converts a object to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this object[,,] value)
    {
        return ToComplex(value, Matrix.CreateAs<object, Complex>(value));
    }

    /// <summary>
    ///   Converts a object to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this object[][] value)
    {
        return ToComplex(value, Jagged.CreateAs<object, Complex>(value));
    }

    /// <summary>
    ///   Converts a object to a 128-bit complex.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this object[][][] value)
    {
        return ToComplex(value, Jagged.CreateAs<object, Complex>(value));
    }

    /// <summary>
    ///   Converts a object array to a 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[] ToComplex(this object[] value, Complex[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Complex)System.Convert.ChangeType(value[i], typeof(Complex));
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional object array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this object[,] value, Complex[,] result)
    {
					int r = value.GetLength(0);
			int c = value.GetLength(1);
			for (var i = 0; i < r; i++)
				for (var j = 0; j < c; j++)
					result[i, j] = (Complex)System.Convert.ChangeType(value[i, j], typeof(Complex));
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional object array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,,] ToComplex(this object[,,] value, Complex[,,] result)
    {
					int r = value.GetLength(0);
			int c = value.GetLength(1);
			int d = value.GetLength(2);
			for (var i = 0; i < r; i++)
				for (var j = 0; j < c; j++)
					for (var k = 0; k < d; k++)
					result[i, j, k] = (Complex)System.Convert.ChangeType(value[i, j, k], typeof(Complex));
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional object array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this object[,] value, Complex[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Complex)System.Convert.ChangeType(value[i, j], typeof(Complex));
        return result;
    }

    /// <summary>
    ///   Converts a jagged object array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][] ToComplex(this object[][] value, Complex[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Complex)System.Convert.ChangeType(value[i][j], typeof(Complex));
        return result;            
    }

    /// <summary>
    ///   Converts a jagged object array to a jagged 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[][][] ToComplex(this object[][][] value, Complex[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Complex)System.Convert.ChangeType(value[i][j][k], typeof(Complex));
        return result;            
    }

    /// <summary>
    ///   Converts a jagged object array to a multidimensional 128-bit complex array.
    /// </summary>
    /// 
    public static Complex[,] ToComplex(this object[][] value, Complex[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Complex)System.Convert.ChangeType(value[i][j], typeof(Complex));
        return result;            
    }
    
    /// <summary>
    ///   Converts a string to a double-precision floating point.
    /// </summary>
    /// 
    public static double[] ToDouble(this string[] value)
    {
        return ToDouble(value, new double[value.Length]);
    }

    /// <summary>
    ///   Converts a string to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,] ToDouble(this string[,] value)
    {
        return ToDouble(value, Matrix.CreateAs<string, double>(value));
    }

		/// <summary>
    ///   Converts a string to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this string[,,] value)
    {
        return ToDouble(value, Matrix.CreateAs<string, double>(value));
    }

    /// <summary>
    ///   Converts a string to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][] ToDouble(this string[][] value)
    {
        return ToDouble(value, Jagged.CreateAs<string, double>(value));
    }

    /// <summary>
    ///   Converts a string to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this string[][][] value)
    {
        return ToDouble(value, Jagged.CreateAs<string, double>(value));
    }

    /// <summary>
    ///   Converts a string array to a double-precision floating point array.
    /// </summary>
    /// 
    public static double[] ToDouble(this string[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = Double.Parse(value[i]);;
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this string[,] value, double[,] result)
    {
					int r = value.GetLength(0);
			int c = value.GetLength(1);
			for (var i = 0; i < r; i++)
				for (var j = 0; j < c; j++)
					result[i, j] = Double.Parse(value[i, j]);;
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional string array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this string[,,] value, double[,,] result)
    {
					int r = value.GetLength(0);
			int c = value.GetLength(1);
			int d = value.GetLength(2);
			for (var i = 0; i < r; i++)
				for (var j = 0; j < c; j++)
					for (var k = 0; k < d; k++)
					result[i, j, k] = Double.Parse(value[i, j, k]);;
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional string array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this string[,] value, double[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = Double.Parse(value[i, j]);;
        return result;
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this string[][] value, double[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = Double.Parse(value[i][j]);;
        return result;            
    }

    /// <summary>
    ///   Converts a jagged string array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this string[][][] value, double[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = Double.Parse(value[i][j][k]);;
        return result;            
    }

    /// <summary>
    ///   Converts a jagged string array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this string[][] value, double[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = Double.Parse(value[i][j]);;
        return result;            
    }
    
    /// <summary>
    ///   Converts a 128-bit complex to a double-precision floating point.
    /// </summary>
    /// 
    public static double[] ToDouble(this Complex[] value)
    {
        return ToDouble(value, new double[value.Length]);
    }

    /// <summary>
    ///   Converts a 128-bit complex to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,] ToDouble(this Complex[,] value)
    {
        return ToDouble(value, Matrix.CreateAs<Complex, double>(value));
    }

		/// <summary>
    ///   Converts a 128-bit complex to a double-precision floating point.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this Complex[,,] value)
    {
        return ToDouble(value, Matrix.CreateAs<Complex, double>(value));
    }

    /// <summary>
    ///   Converts a 128-bit complex to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][] ToDouble(this Complex[][] value)
    {
        return ToDouble(value, Jagged.CreateAs<Complex, double>(value));
    }

    /// <summary>
    ///   Converts a 128-bit complex to a double-precision floating point.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this Complex[][][] value)
    {
        return ToDouble(value, Jagged.CreateAs<Complex, double>(value));
    }

    /// <summary>
    ///   Converts a 128-bit complex array to a double-precision floating point array.
    /// </summary>
    /// 
    public static double[] ToDouble(this Complex[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
            result[i] = (Double)value[i].Real;
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional 128-bit complex array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this Complex[,] value, double[,] result)
    {
					unsafe
			{
				fixed (Complex* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i].Real;
				}
			}
		
        return result;
    }

		/// <summary>
    ///   Converts a multidimensional 128-bit complex array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,,] ToDouble(this Complex[,,] value, double[,,] result)
    {
					unsafe
			{
				fixed (Complex* src = value)
				fixed (double* dst = result)
				{
					for (var i = 0; i < value.Length; i++)
						dst[i] = (Double)src[i].Real;
				}
			}
		
        return result;
    }

    /// <summary>
    ///   Converts a multidimensional 128-bit complex array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this Complex[,] value, double[][] result)
    {
                    for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (Double)value[i, j].Real;
        return result;
    }

    /// <summary>
    ///   Converts a jagged 128-bit complex array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][] ToDouble(this Complex[][] value, double[][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i][j] = (Double)value[i][j].Real;
        return result;            
    }

    /// <summary>
    ///   Converts a jagged 128-bit complex array to a jagged double-precision floating point array.
    /// </summary>
    /// 
    public static double[][][] ToDouble(this Complex[][][] value, double[][][] result)
    {
        for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                for (var k = 0; k < value[i][j].Length; k++)
                    result[i][j][k] = (Double)value[i][j][k].Real;
        return result;            
    }

    /// <summary>
    ///   Converts a jagged 128-bit complex array to a multidimensional double-precision floating point array.
    /// </summary>
    /// 
    public static double[,] ToDouble(this Complex[][] value, double[,] result)
    {
                    for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].Length; j++)
                result[i, j] = (Double)value[i][j].Real;
        return result;            
    }
}