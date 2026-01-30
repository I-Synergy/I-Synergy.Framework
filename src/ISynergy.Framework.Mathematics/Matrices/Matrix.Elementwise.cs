namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Matrix
{
    /// <summary>
    ///     Elementwise multiply operation.
    /// </summary>
    public static double[,] ElementwiseMultiply(double[,] a, double[,] b)
    {
        return Elementwise.Multiply(a, b);
    }

    /// <summary>
    ///   Elementwise divide operation.
    /// </summary>
    public static double[,] ElementwiseDivide(this double[,] a, double[,] b)
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
        return Exp(value, MatrixCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise exponential.
    /// </summary>
    /// 
    public static double[][] Exp(this double[][] value)
    {
        return Exp(value, JaggedCreateAs<double, double>(value));
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
        return Log(value, MatrixCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise logarithm.
    /// </summary>
    /// 
    public static double[][] Log(this double[][] value)
    {
        return Log(value, JaggedCreateAs<double, double>(value));
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
        return Sign(value, MatrixCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise sign.
    /// </summary>
    /// 
    public static double[][] Sign(this double[][] value)
    {
        return Sign(value, JaggedCreateAs<double, double>(value));
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
        return Abs(value, MatrixCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise absolute value.
    /// </summary>
    /// 
    public static double[][] Abs(this double[][] value)
    {
        return Abs(value, JaggedCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    /// 
    public static double[,] Sqrt(this double[,] value)
    {
        return Sqrt(value, MatrixCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    /// 
    public static double[][] Sqrt(this double[][] value)
    {
        return Sqrt(value, JaggedCreateAs<double, double>(value));
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
        return SignSqrt(value, MatrixCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise signed square-root.
    /// </summary>
    /// 
    public static double[][] SignSqrt(this double[][] value)
    {
        return SignSqrt(value, JaggedCreateAs<double, double>(value));
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
        return Floor(value, MatrixCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise floor.
    /// </summary>
    /// 
    public static double[][] Floor(this double[][] value)
    {
        return Floor(value, JaggedCreateAs<double, double>(value));
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
        return Ceiling(value, MatrixCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise ceiling.
    /// </summary>
    /// 
    public static double[][] Ceiling(this double[][] value)
    {
        return Ceiling(value, JaggedCreateAs<double, double>(value));
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
        return Round(value, MatrixCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise round.
    /// </summary>
    /// 
    public static double[][] Round(this double[][] value)
    {
        return Round(value, JaggedCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise signed power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// 
    public static double[,] SignedPow(this double[,] value, double y)
    {
        return SignedPow(value, y, MatrixCreateAs<double, double>(value));
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
        return SignedPow(value, y, JaggedCreateAs<double, double>(value));
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
        return Pow(value, y, MatrixCreateAs<double, double>(value));
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
        return Pow(value, y, JaggedCreateAs<double, double>(value));
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
    public static double[,] SignedPow(this double[,] value, double y, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (var j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (Math.Sign(v) * Math.Pow(Math.Abs(v), y));
                }
            }
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
    public static double[][] SignedPow(this double[][] value, double y, double[][] result)
    {
        unsafe
        {
            for (var i = 0; i < value.Length; i++)
            {
                for (var j = 0; j < value[i].Length; j++)
                {
                    var v = value[i][j];
                    result[i][j] = (Math.Sign(v) * Math.Pow(Math.Abs(v), y));
                }
            }
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
    public static double[,] Pow(this double[,] value, double y, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (var j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (Math.Pow(Math.Abs(v), y));
                }
            }
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
    public static double[][] Pow(this double[][] value, double y, double[][] result)
    {
        unsafe
        {
            for (var i = 0; i < value.Length; i++)
            {
                for (var j = 0; j < value[i].Length; j++)
                {
                    var v = value[i][j];
                    result[i][j] = (Math.Pow(Math.Abs(v), y));
                }
            }
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
    public static double[] Exp(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (double)(Math.Exp((double)v));
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
    public static double[,] Exp(this double[,] value, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (var j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (double)(Math.Exp((double)v));
                }
            }
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
    public static double[][] Exp(this double[][] value, double[][] result)
    {
        unsafe
        {
            for (var i = 0; i < value.Length; i++)
            {
                for (var j = 0; j < value[i].Length; j++)
                {
                    var v = value[i][j];
                    result[i][j] = (double)(Math.Exp((double)v));
                }
            }
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
    public static double[] Log(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (double)(Math.Log((double)v));
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
    public static double[,] Log(this double[,] value, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (var j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (double)(Math.Log((double)v));
                }
            }
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
    public static double[][] Log(this double[][] value, double[][] result)
    {
        unsafe
        {
            for (var i = 0; i < value.Length; i++)
            {
                for (var j = 0; j < value[i].Length; j++)
                {
                    var v = value[i][j];
                    result[i][j] = (double)(Math.Log((double)v));
                }
            }
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
    public static double[,] Sign(this double[,] value, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (var j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (double)(Math.Sign(v));
                }
            }
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
    public static double[][] Sign(this double[][] value, double[][] result)
    {
        unsafe
        {
            for (var i = 0; i < value.Length; i++)
            {
                for (var j = 0; j < value[i].Length; j++)
                {
                    var v = value[i][j];
                    result[i][j] = (double)(Math.Sign(v));
                }
            }
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
    public static double[] Abs(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (double)(Math.Abs(v));
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
    public static double[,] Abs(this double[,] value, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (var j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (double)(Math.Abs(v));
                }
            }
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
    public static double[][] Abs(this double[][] value, double[][] result)
    {
        unsafe
        {
            for (var i = 0; i < value.Length; i++)
            {
                for (var j = 0; j < value[i].Length; j++)
                {
                    var v = value[i][j];
                    result[i][j] = (double)(Math.Abs(v));
                }
            }
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
    public static double[] Sqrt(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (double)(Math.Sqrt((double)v));
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
    public static double[,] Sqrt(this double[,] value, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (var j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (double)(Math.Sqrt((double)v));
                }
            }
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
    public static double[][] Sqrt(this double[][] value, double[][] result)
    {
        unsafe
        {
            for (var i = 0; i < value.Length; i++)
            {
                for (var j = 0; j < value[i].Length; j++)
                {
                    var v = value[i][j];
                    result[i][j] = (double)(Math.Sqrt((double)v));
                }
            }
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
    public static double[] SignSqrt(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (double)(Math.Sign(v) * Math.Sqrt((double)v));
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
    public static double[,] SignSqrt(this double[,] value, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (var j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (double)(Math.Sign(v) * Math.Sqrt((double)v));
                }
            }
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
    public static double[][] SignSqrt(this double[][] value, double[][] result)
    {
        unsafe
        {
            for (var i = 0; i < value.Length; i++)
            {
                for (var j = 0; j < value[i].Length; j++)
                {
                    var v = value[i][j];
                    result[i][j] = (double)(Math.Sign(v) * Math.Sqrt((double)v));
                }
            }
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
    public static double[] Floor(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (double)(Math.Floor((double)v));
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
    public static double[,] Floor(this double[,] value, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (var j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (double)(Math.Floor((double)v));
                }
            }
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
    public static double[][] Floor(this double[][] value, double[][] result)
    {
        unsafe
        {
            for (var i = 0; i < value.Length; i++)
            {
                for (var j = 0; j < value[i].Length; j++)
                {
                    var v = value[i][j];
                    result[i][j] = (double)(Math.Floor((double)v));
                }
            }
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
    public static double[] Ceiling(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (double)(Math.Ceiling((double)v));
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
    public static double[,] Ceiling(this double[,] value, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (var j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (double)(Math.Ceiling((double)v));
                }
            }
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
    public static double[][] Ceiling(this double[][] value, double[][] result)
    {
        unsafe
        {
            for (var i = 0; i < value.Length; i++)
            {
                for (var j = 0; j < value[i].Length; j++)
                {
                    var v = value[i][j];
                    result[i][j] = (double)(Math.Ceiling((double)v));
                }
            }
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
    public static double[] Round(this double[] value, double[] result)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (double)(Math.Round((double)v));
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
    public static double[,] Round(this double[,] value, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (var j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (double)(Math.Round((double)v));
                }
            }
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
    public static double[][] Round(this double[][] value, double[][] result)
    {
        unsafe
        {
            for (var i = 0; i < value.Length; i++)
            {
                for (var j = 0; j < value[i].Length; j++)
                {
                    var v = value[i][j];
                    result[i][j] = (double)(Math.Round((double)v));
                }
            }
        }
        return result;
    }

    private static TOutput[] VectorCreateAs<TInput, TOutput>(TInput[] vector)
    {
        return new TOutput[vector.Length];
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

    private static TOutput[,] MatrixCreateAs<TInput, TOutput>(TInput[][] matrix)
    {
        return new TOutput[matrix.Length, matrix[0].Length];
    }

    private static TOutput[][] JaggedCreateAs<TInput, TOutput>(TInput[,] matrix)
    {
        var r = new TOutput[matrix.GetLength(0)][];
        for (int i = 0; i < r.Length; i++)
            r[i] = new TOutput[matrix.GetLength(1)];
        return r;
    }
}