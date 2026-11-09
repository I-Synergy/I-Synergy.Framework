using System.Runtime.InteropServices;



using ISynergy.Framework.Mathematics.Abstractions;
namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Matrix
{
    /// <summary>
    ///     Elementwise multiply operation.
    /// </summary>
    public static double[,] ElementwiseMultiply(Matrix<double> a, Matrix<double> b)
    {
        return Elementwise.Multiply(a, b);
    }

    /// <summary>
    ///   Elementwise divide operation.
    /// </summary>
    public static double[,] ElementwiseDivide(this Matrix<double> a, Matrix<double> b)
    {
        return Elementwise.Divide(a, b);
    }

    /// <summary>
    ///   Elementwise exponential.
    /// </summary>
    /// 
    public static double[] Exp(this double[] value)
    {
        return Exp(value, new double[value.Length]);
    }

    /// <summary>
    ///   Elementwise exponential.
    /// </summary>
    /// 
    public static double[,] Exp(this double[,] value)
    {
        return Exp(new Matrix<double>(value), new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise exponential.
    /// </summary>
    /// 
    public static double[][] Exp(this double[][] value)
    {
        return Exp(new Jagged<double>(value), new Jagged<double>(JaggedCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise exponential.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] Exp(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = Math.Exp(v);
        }

        return result;
    }

    /// <summary>
    ///   Elementwise exponential.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Exp(this Matrix<double> value, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        if (valueArray.Length == 0)
            return resultArray;
        var spanV = MemoryMarshal.CreateSpan(ref valueArray[0, 0], valueArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var j = 0; j < spanV.Length; j++)
        {
            var v = spanV[j];
            spanR[j] = Math.Exp(v);
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise exponential.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    ///
    public static double[][] Exp(this Jagged<double> value, Jagged<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        for (var i = 0; i < valueArray.Length; i++)
        {
            for (var j = 0; j < valueArray[i].Length; j++)
            {
                var v = valueArray[i][j];
                resultArray[i][j] = Math.Exp(v);
            }
        }
        return resultArray;
    }

    /// <summary>
    ///   Elementwise logarithm.
    /// </summary>
    /// 
    public static double[] Log(this double[] value)
    {
        return Log(value, new double[value.Length]);
    }

    /// <summary>
    ///   Elementwise logarithm.
    /// </summary>
    /// 
    public static double[,] Log(this double[,] value)
    {
        return Log(new Matrix<double>(value), new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise logarithm.
    /// </summary>
    /// 
    public static double[][] Log(this double[][] value)
    {
        return Log(new Jagged<double>(value), new Jagged<double>(JaggedCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise logarithm.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] Log(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = Math.Log(v);
        }

        return result;
    }

    /// <summary>
    ///   Elementwise logarithm.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Log(this Matrix<double> value, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        if (valueArray.Length == 0)
            return resultArray;
        var spanV = MemoryMarshal.CreateSpan(ref valueArray[0, 0], valueArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var j = 0; j < spanV.Length; j++)
        {
            var v = spanV[j];
            spanR[j] = Math.Log(v);
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise logarithm.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    ///
    public static double[][] Log(this Jagged<double> value, Jagged<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        for (var i = 0; i < valueArray.Length; i++)
        {
            for (var j = 0; j < valueArray[i].Length; j++)
            {
                var v = valueArray[i][j];
                resultArray[i][j] = Math.Log(v);
            }
        }
        return resultArray;
    }

    /// <summary>
    ///   Elementwise sign.
    /// </summary>
    /// 
    public static double[] Sign(this double[] value)
    {
        return Sign(value, new double[value.Length]);
    }

    /// <summary>
    ///   Elementwise sign.
    /// </summary>
    /// 
    public static double[,] Sign(this double[,] value)
    {
        return Sign(new Matrix<double>(value), new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise sign.
    /// </summary>
    /// 
    public static double[][] Sign(this double[][] value)
    {
        return Sign(new Jagged<double>(value), new Jagged<double>(JaggedCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise sign.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] Sign(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (double)(Math.Sign(v));
        }

        return result;
    }

    /// <summary>
    ///   Elementwise sign.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Sign(this Matrix<double> value, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        if (valueArray.Length == 0)
            return resultArray;
        var spanV = MemoryMarshal.CreateSpan(ref valueArray[0, 0], valueArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var j = 0; j < spanV.Length; j++)
        {
            var v = spanV[j];
            spanR[j] = (double)(Math.Sign(v));
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise sign.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    ///
    public static double[][] Sign(this Jagged<double> value, Jagged<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        for (var i = 0; i < valueArray.Length; i++)
        {
            for (var j = 0; j < valueArray[i].Length; j++)
            {
                var v = valueArray[i][j];
                resultArray[i][j] = (double)(Math.Sign(v));
            }
        }
        return resultArray;
    }

    /// <summary>
    ///   Elementwise absolute value.
    /// </summary>
    /// 
    public static double[] Abs(this double[] value)
    {
        return Abs(value, new double[value.Length]);
    }

    /// <summary>
    ///   Elementwise absolute value.
    /// </summary>
    /// 
    public static double[,] Abs(this double[,] value)
    {
        return Abs(new Matrix<double>(value), new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise absolute value.
    /// </summary>
    /// 
    public static double[][] Abs(this double[][] value)
    {
        return Abs(new Jagged<double>(value), new Jagged<double>(JaggedCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise absolute value.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] Abs(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = Math.Abs(v);
        }

        return result;
    }

    /// <summary>
    ///   Elementwise absolute value.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Abs(this Matrix<double> value, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        if (valueArray.Length == 0)
            return resultArray;
        var spanV = MemoryMarshal.CreateSpan(ref valueArray[0, 0], valueArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var j = 0; j < spanV.Length; j++)
        {
            var v = spanV[j];
            spanR[j] = Math.Abs(v);
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise absolute value.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    ///
    public static double[][] Abs(this Jagged<double> value, Jagged<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        for (var i = 0; i < valueArray.Length; i++)
        {
            for (var j = 0; j < valueArray[i].Length; j++)
            {
                var v = valueArray[i][j];
                resultArray[i][j] = Math.Abs(v);
            }
        }
        return resultArray;
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    /// 
    public static double[,] Sqrt(this double[,] value)
    {
        return Sqrt(new Matrix<double>(value), new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    /// 
    public static double[][] Sqrt(this double[][] value)
    {
        return Sqrt(new Jagged<double>(value), new Jagged<double>(JaggedCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] Sqrt(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = Math.Sqrt(v);
        }

        return result;
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Sqrt(this Matrix<double> value, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        if (valueArray.Length == 0)
            return resultArray;
        var spanV = MemoryMarshal.CreateSpan(ref valueArray[0, 0], valueArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var j = 0; j < spanV.Length; j++)
        {
            var v = spanV[j];
            spanR[j] = Math.Sqrt(v);
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    ///
    public static double[][] Sqrt(this Jagged<double> value, Jagged<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        for (var i = 0; i < valueArray.Length; i++)
        {
            for (var j = 0; j < valueArray[i].Length; j++)
            {
                var v = valueArray[i][j];
                resultArray[i][j] = Math.Sqrt(v);
            }
        }
        return resultArray;
    }

    /// <summary>
    ///   Elementwise signed square-root.
    /// </summary>
    /// 
    public static double[] SignSqrt(this double[] value)
    {
        return SignSqrt(value, new double[value.Length]);
    }

    /// <summary>
    ///   Elementwise signed square-root.
    /// </summary>
    /// 
    public static double[,] SignSqrt(this double[,] value)
    {
        return SignSqrt(new Matrix<double>(value), new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise signed square-root.
    /// </summary>
    /// 
    public static double[][] SignSqrt(this double[][] value)
    {
        return SignSqrt(new Jagged<double>(value), new Jagged<double>(JaggedCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise signed square-root.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] SignSqrt(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = Math.Sign(v) * Math.Sqrt(v);
        }

        return result;
    }

    /// <summary>
    ///   Elementwise signed square-root.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] SignSqrt(this Matrix<double> value, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        if (valueArray.Length == 0)
            return resultArray;
        var spanV = MemoryMarshal.CreateSpan(ref valueArray[0, 0], valueArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var j = 0; j < spanV.Length; j++)
        {
            var v = spanV[j];
            spanR[j] = Math.Sign(v) * Math.Sqrt(v);
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise signed square-root.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    ///
    public static double[][] SignSqrt(this Jagged<double> value, Jagged<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        for (var i = 0; i < valueArray.Length; i++)
        {
            for (var j = 0; j < valueArray[i].Length; j++)
            {
                var v = valueArray[i][j];
                resultArray[i][j] = Math.Sign(v) * Math.Sqrt(v);
            }
        }
        return resultArray;
    }

    /// <summary>
    ///   Elementwise floor.
    /// </summary>
    /// 
    public static double[] Floor(this double[] value)
    {
        return Floor(value, new double[value.Length]);
    }

    /// <summary>
    ///   Elementwise floor.
    /// </summary>
    /// 
    public static double[,] Floor(this double[,] value)
    {
        return Floor(new Matrix<double>(value), new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise floor.
    /// </summary>
    /// 
    public static double[][] Floor(this double[][] value)
    {
        return Floor(new Jagged<double>(value), new Jagged<double>(JaggedCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise floor.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] Floor(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = Math.Floor(v);
        }

        return result;
    }

    /// <summary>
    ///   Elementwise floor.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Floor(this Matrix<double> value, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        if (valueArray.Length == 0)
            return resultArray;
        var spanV = MemoryMarshal.CreateSpan(ref valueArray[0, 0], valueArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var j = 0; j < spanV.Length; j++)
        {
            var v = spanV[j];
            spanR[j] = Math.Floor(v);
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise floor.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    ///
    public static double[][] Floor(this Jagged<double> value, Jagged<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        for (var i = 0; i < valueArray.Length; i++)
        {
            for (var j = 0; j < valueArray[i].Length; j++)
            {
                var v = valueArray[i][j];
                resultArray[i][j] = Math.Floor(v);
            }
        }
        return resultArray;
    }

    /// <summary>
    ///   Elementwise ceiling.
    /// </summary>
    /// 
    public static double[] Ceiling(this double[] value)
    {
        return Ceiling(value, new double[value.Length]);
    }

    /// <summary>
    ///   Elementwise ceiling.
    /// </summary>
    /// 
    public static double[,] Ceiling(this double[,] value)
    {
        return Ceiling(new Matrix<double>(value), new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise ceiling.
    /// </summary>
    /// 
    public static double[][] Ceiling(this double[][] value)
    {
        return Ceiling(new Jagged<double>(value), new Jagged<double>(JaggedCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise ceiling.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] Ceiling(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = Math.Ceiling(v);
        }

        return result;
    }

    /// <summary>
    ///   Elementwise ceiling.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Ceiling(this Matrix<double> value, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        if (valueArray.Length == 0)
            return resultArray;
        var spanV = MemoryMarshal.CreateSpan(ref valueArray[0, 0], valueArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var j = 0; j < spanV.Length; j++)
        {
            var v = spanV[j];
            spanR[j] = Math.Ceiling(v);
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise ceiling.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    ///
    public static double[][] Ceiling(this Jagged<double> value, Jagged<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        for (var i = 0; i < valueArray.Length; i++)
        {
            for (var j = 0; j < valueArray[i].Length; j++)
            {
                var v = valueArray[i][j];
                resultArray[i][j] = Math.Ceiling(v);
            }
        }
        return resultArray;
    }

    /// <summary>
    ///   Elementwise round.
    /// </summary>
    /// 
    public static double[] Round(this double[] value)
    {
        return Round(value, new double[value.Length]);
    }

    /// <summary>
    ///   Elementwise round.
    /// </summary>
    /// 
    public static double[,] Round(this double[,] value)
    {
        return Round(new Matrix<double>(value), new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise round.
    /// </summary>
    /// 
    public static double[][] Round(this double[][] value)
    {
        return Round(new Jagged<double>(value), new Jagged<double>(JaggedCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise round.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] Round(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = Math.Round(v);
        }

        return result;
    }

    /// <summary>
    ///   Elementwise round.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Round(this Matrix<double> value, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        if (valueArray.Length == 0)
            return resultArray;
        var spanV = MemoryMarshal.CreateSpan(ref valueArray[0, 0], valueArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var j = 0; j < spanV.Length; j++)
        {
            var v = spanV[j];
            spanR[j] = Math.Round(v);
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise round.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    ///
    public static double[][] Round(this Jagged<double> value, Jagged<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        for (var i = 0; i < valueArray.Length; i++)
        {
            for (var j = 0; j < valueArray[i].Length; j++)
            {
                var v = valueArray[i][j];
                resultArray[i][j] = Math.Round(v);
            }
        }
        return resultArray;
    }

    /// <summary>
    ///   Elementwise signed power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// 
    public static double[,] SignedPow(this double[,] value, double y) // NOSONAR
    {
        return SignedPow(new Matrix<double>(value), y, new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise signed power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// 
    public static double[] SignedPow(this double[] value, double y)
    {
        return SignedPow(value, y, new double[value.Length]);
    }

    /// <summary>
    ///   Elementwise signed power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// 
    public static double[][] SignedPow(this double[][] value, double y)
    {
        return SignedPow(new Jagged<double>(value), y, new Jagged<double>(JaggedCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise signed power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] SignedPow(this double[] value, double y, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (Math.Sign(v) * Math.Pow(Math.Abs(v), y));
        }

        return result;
    }

    /// <summary>
    ///   Elementwise signed power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] SignedPow(this Matrix<double> value, double y, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        if (valueArray.Length == 0)
            return resultArray;
        var spanV = MemoryMarshal.CreateSpan(ref valueArray[0, 0], valueArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var j = 0; j < spanV.Length; j++)
        {
            var v = spanV[j];
            spanR[j] = (Math.Sign(v) * Math.Pow(Math.Abs(v), y));
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise signed power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    ///
    public static double[][] SignedPow(this Jagged<double> value, double y, Jagged<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        for (var i = 0; i < valueArray.Length; i++)
        {
            for (var j = 0; j < valueArray[i].Length; j++)
            {
                var v = valueArray[i][j];
                resultArray[i][j] = (Math.Sign(v) * Math.Pow(Math.Abs(v), y));
            }
        }
        return resultArray;
    }

    /// <summary>
    ///   Elementwise power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// 
    public static double[] Pow(this double[] value, double y)
    {
        return Pow(value, y, new double[value.Length]);
    }

    /// <summary>
    ///   Elementwise power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// 
    public static double[,] Pow(this double[,] value, double y)
    {
        return Pow(new Matrix<double>(value), y, new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// 
    public static double[][] Pow(this double[][] value, double y)
    {
        return Pow(new Jagged<double>(value), y, new Jagged<double>(JaggedCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] Pow(this double[] value, double y, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (Math.Pow(Math.Abs(v), y));
        }

        return result;
    }

    /// <summary>
    ///   Elementwise power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Pow(this Matrix<double> value, double y, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        if (valueArray.Length == 0)
            return resultArray;
        var spanV = MemoryMarshal.CreateSpan(ref valueArray[0, 0], valueArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var j = 0; j < spanV.Length; j++)
        {
            var v = spanV[j];
            spanR[j] = (Math.Pow(Math.Abs(v), y));
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    ///
    public static double[][] Pow(this Jagged<double> value, double y, Jagged<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        for (var i = 0; i < valueArray.Length; i++)
        {
            for (var j = 0; j < valueArray[i].Length; j++)
            {
                var v = valueArray[i][j];
                resultArray[i][j] = (Math.Pow(Math.Abs(v), y));
            }
        }
        return resultArray;
    }


    private static TOutput[,] MatrixCreateAs<TInput, TOutput>(TInput[,] matrix)
    {
        return new TOutput[matrix.GetLength(0), matrix.GetLength(1)];
    }

    private static TOutput[][] JaggedCreateAs<TInput, TOutput>(TInput[][] matrix)
    {
        var r = new TOutput[matrix.Length][];
        for (int i = 0; i < r.Length; i++)
            r[i] = new TOutput[matrix[i].Length];
        return r;
    }
}