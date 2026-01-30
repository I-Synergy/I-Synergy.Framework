using ISynergy.Framework.Core.Points;

namespace ISynergy.Framework.Mathematics.Common;

/// <summary>
///     Set of mathematical tools.
/// </summary>
public static class Tools
{
    /// <summary>
    ///     Gets the angle formed by the vector [x,y].
    /// </summary>
    public static float Angle(float x, float y)
    {
        if (y >= 0)
        {
            if (x >= 0)
                return (float)Math.Atan(y / x);
            return (float)(Math.PI - Math.Atan(-y / x));
        }

        if (x >= 0)
            return (float)(2 * Math.PI - Math.Atan(-y / x));
        return (float)(Math.PI + Math.Atan(y / x));
    }

    /// <summary>
    ///     Gets the angle formed by the vector [x,y].
    /// </summary>
    public static double Angle(double x, double y)
    {
        if (y >= 0)
        {
            if (x >= 0)
                return Math.Atan2(y, x);
            return Math.PI - Math.Atan(-y / x);
        }

        if (x >= 0)
            return 2.0 * Math.PI - Math.Atan2(-y, x);
        return Math.PI + Math.Atan(y / x);
    }

    /// <summary>
    ///     Gets the displacement angle between two points.
    /// </summary>
    public static double Angle(Point previous, Point next)
    {
        double dx = next.X - previous.X;
        double dy = next.Y - previous.Y;

        return Angle(dx, dy);
    }

    /// <summary>
    ///     Gets the displacement angle between two points, coded
    ///     as an integer varying from 0 to 20.
    /// </summary>
    public static int Direction(Point previous, Point next)
    {
        double dx = next.X - previous.X;
        double dy = next.Y - previous.Y;

        var radians = Angle(dx, dy);

        // code = Floor(20 / (2*System.Math.PI))
        var code = (int)Math.Floor(radians * 3.183098861837907);

        return code;
    }

    /// <summary>
    ///     Gets the greatest common divisor between two integers.
    /// </summary>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    /// <returns>The greatest common divisor.</returns>
    public static int GreatestCommonDivisor(int a, int b)
    {
        var x = a - b * (int)Math.Floor(a / (double)b);
        while (x != 0)
        {
            a = b;
            b = x;
            x = a - b * (int)Math.Floor(a / (double)b);
        }

        return b;
    }

    /// <summary>
    ///     Returns the next power of 2 after the input value x.
    /// </summary>
    /// <param name="x">Input value x.</param>
    /// <returns>Returns the next power of 2 after the input value x.</returns>
    public static int NextPowerOf2(int x)
    {
        --x;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        return ++x;
    }

    /// <summary>
    ///     Returns the previous power of 2 after the input value x.
    /// </summary>
    /// <param name="x">Input value x.</param>
    /// <returns>Returns the previous power of 2 after the input value x.</returns>
    public static int PreviousPowerOf2(int x)
    {
        return NextPowerOf2(x + 1) / 2;
    }
    /// <summary>
    ///     Hypotenuse calculus without overflow/underflow
    /// </summary>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    /// <returns>The hypotenuse Sqrt(a^2 + b^2)</returns>
    public static double Hypotenuse(double a, double b)
    {
        var r = 0.0;
        var absA = Math.Abs(a);
        var absB = Math.Abs(b);

        if (absA > absB)
        {
            r = b / a;
            r = absA * Math.Sqrt(1 + r * r);
        }
        else if (b != 0)
        {
            r = a / b;
            r = absB * Math.Sqrt(1 + r * r);
        }

        return r;
    }

    /// <summary>
    ///     Hypotenuse calculus without overflow/underflow
    /// </summary>
    /// <param name="a">first value</param>
    /// <param name="b">second value</param>
    /// <returns>The hypotenuse Sqrt(a^2 + b^2)</returns>
    public static decimal Hypotenuse(decimal a, decimal b)
    {
        decimal r = 0;
        var absA = Math.Abs(a);
        var absB = Math.Abs(b);

        if (absA > absB)
        {
            r = b / a;
            r = absA * (decimal)Math.Sqrt((double)(1 + r * r));
        }
        else if (b != 0)
        {
            r = a / b;
            r = absB * (decimal)Math.Sqrt((double)(1 + r * r));
        }

        return r;
    }

    /// <summary>
    ///     Hypotenuse calculus without overflow/underflow
    /// </summary>
    /// <param name="a">first value</param>
    /// <param name="b">second value</param>
    /// <returns>The hypotenuse Sqrt(a^2 + b^2)</returns>
    public static float Hypotenuse(float a, float b)
    {
        double r = 0;
        var absA = Math.Abs(a);
        var absB = Math.Abs(b);

        if (absA > absB)
        {
            r = b / a;
            r = absA * Math.Sqrt(1 + r * r);
        }
        else if (b != 0)
        {
            r = a / b;
            r = absB * Math.Sqrt(1 + r * r);
        }

        return (float)r;
    }

    /// <summary>
    ///     Gets the proper modulus operation for
    ///     an integer value x and modulo m.
    /// </summary>
    public static int Mod(int x, int m)
    {
        if (m < 0)
            m = -m;

        var r = x % m;

        return r < 0 ? r + m : r;
    }

    /// <summary>
    ///     Gets the proper modulus operation for
    ///     a real value x and modulo m.
    /// </summary>
    public static double Mod(double x, double m)
    {
        if (m < 0)
            m = -m;

        var r = x % m;

        return r < 0 ? r + m : r;
    }
    /// <summary>
    ///     Returns the hyperbolic arc cosine of the specified value.
    /// </summary>
    public static double Acosh(double x)
    {
        if (x < 1.0)
            throw new ArgumentOutOfRangeException("x");
        return Math.Log(x + Math.Sqrt(x * x - 1));
    }

    /// <summary>
    ///     Returns the hyperbolic arc sine of the specified value.
    /// </summary>
    public static double Asinh(double d)
    {
        double x;
        int sign;

        if (d == 0.0)
            return d;

        if (d < 0.0)
        {
            sign = -1;
            x = -d;
        }
        else
        {
            sign = 1;
            x = d;
        }

        return sign * Math.Log(x + Math.Sqrt(x * x + 1));
    }

    /// <summary>
    ///     Returns the hyperbolic arc tangent of the specified value.
    /// </summary>
    public static double Atanh(double d)
    {
        if (d > 1.0 || d < -1.0)
            throw new ArgumentOutOfRangeException("d");
        return 0.5 * Math.Log((1.0 + d) / (1.0 - d));
    }
    /// <summary>
    ///     Returns the factorial falling power of the specified value.
    /// </summary>
    public static int FactorialPower(int value, int degree)
    {
        var t = value;
        for (var i = 0; i < degree; i++)
            t *= degree--;
        return t;
    }

    /// <summary>
    ///     Truncated power function.
    /// </summary>
    public static double TruncatedPower(double value, double degree)
    {
        var x = Math.Pow(value, degree);
        return x > 0 ? x : 0.0;
    }

    /// <summary>
    ///     Fast inverse floating-point square root.
    /// </summary>
    public static float InvSqrt(float f)
    {
        unsafe
        {
            var xhalf = 0.5f * f;
            var i = *(int*)&f;
            i = 0x5f375a86 - (i >> 1);
            f = *(float*)&i;
            f = f * (1.5f - xhalf * f * f);
            return f;
        }
    }
    /// <summary>
    ///     Interpolates data using a piece-wise linear function.
    /// </summary>
    /// <param name="value">The value to be calculated.</param>
    /// <param name="x">The input data points <c>x</c>. Those values need to be sorted.</param>
    /// <param name="y">The output data points <c>y</c>.</param>
    /// <param name="lower">
    ///     The value to be returned for values before the first point in <paramref name="x" />.
    /// </param>
    /// <param name="upper">
    ///     The value to be returned for values after the last point in <paramref name="x" />.
    /// </param>
    /// <returns>
    ///     Computes the output for f(value) by using a piecewise linear
    ///     interpolation of the data points <paramref name="x" /> and <paramref name="y" />.
    /// </returns>
    public static double Interpolate1D(double value, double[] x, double[] y, double lower, double upper)
    {
        for (var i = 0; i < x.Length; i++)
            if (value < x[i])
            {
                if (i == 0)
                    return lower;

                var start = i - 1;
                var next = i;

                var m = (value - x[start]) / (x[next] - x[start]);
                return y[start] + (y[next] - y[start]) * m;
            }

        return upper;
    }

    /// <summary>
    ///     Gets the maximum value among three values.
    /// </summary>
    /// <param name="a">The first value <c>a</c>.</param>
    /// <param name="b">The second value <c>b</c>.</param>
    /// <param name="c">The third value <c>c</c>.</param>
    /// <returns>
    ///     The maximum value among <paramref name="a" />,
    ///     <paramref name="b" /> and <paramref name="c" />.
    /// </returns>
    public static double Max(double a, double b, double c)
    {
        if (a > b)
        {
            if (c > a)
                return c;
            return a;
        }

        if (c > b)
            return c;
        return b;
    }

    /// <summary>
    ///     Gets the minimum value among three values.
    /// </summary>
    /// <param name="a">The first value <c>a</c>.</param>
    /// <param name="b">The second value <c>b</c>.</param>
    /// <param name="c">The third value <c>c</c>.</param>
    /// <returns>
    ///     The minimum value among <paramref name="a" />,
    ///     <paramref name="b" /> and <paramref name="c" />.
    /// </returns>
    public static double Min(double a, double b, double c)
    {
        if (a < b)
        {
            if (c < a)
                return c;
            return a;
        }

        if (c < b)
            return c;
        return b;
    }

    /// <summary>
    ///     Calculates power of 2.
    /// </summary>
    /// <param name="power">Power to raise in.</param>
    /// <returns>
    ///     Returns specified power of 2 in the case if power is in the range of
    ///     [0, 30]. Otherwise returns 0.
    /// </returns>
    public static int Pow2(int power)
    {
        return power >= 0 && power <= 30 ? 1 << power : 0;
    }

    /// <summary>
    ///     Checks if the specified integer is power of 2.
    /// </summary>
    /// <param name="x">Integer number to check.</param>
    /// <returns>
    ///     Returns <b>true</b> if the specified number is power of 2.
    ///     Otherwise returns <b>false</b>.
    /// </returns>
    public static bool IsPowerOf2(int x)
    {
        return x > 0 ? (x & x - 1) == 0 : false;
    }

    /// <summary>
    ///     Get base of binary logarithm.
    /// </summary>
    /// <param name="x">Source integer number.</param>
    /// <returns>Power of the number (base of binary logarithm).</returns>
    public static int Log2(int x)
    {
        if (x <= 65536)
        {
            if (x <= 256)
            {
                if (x <= 16)
                {
                    if (x <= 4)
                    {
                        if (x <= 2)
                        {
                            if (x <= 1)
                                return 0;
                            return 1;
                        }

                        return 2;
                    }

                    if (x <= 8)
                        return 3;
                    return 4;
                }

                if (x <= 64)
                {
                    if (x <= 32)
                        return 5;
                    return 6;
                }

                if (x <= 128)
                    return 7;
                return 8;
            }

            if (x <= 4096)
            {
                if (x <= 1024)
                {
                    if (x <= 512)
                        return 9;
                    return 10;
                }

                if (x <= 2048)
                    return 11;
                return 12;
            }

            if (x <= 16384)
            {
                if (x <= 8192)
                    return 13;
                return 14;
            }

            if (x <= 32768)
                return 15;
            return 16;
        }

        if (x <= 16777216)
        {
            if (x <= 1048576)
            {
                if (x <= 262144)
                {
                    if (x <= 131072)
                        return 17;
                    return 18;
                }

                if (x <= 524288)
                    return 19;
                return 20;
            }

            if (x <= 4194304)
            {
                if (x <= 2097152)
                    return 21;
                return 22;
            }

            if (x <= 8388608)
                return 23;
            return 24;
        }

        if (x <= 268435456)
        {
            if (x <= 67108864)
            {
                if (x <= 33554432)
                    return 25;
                return 26;
            }

            if (x <= 134217728)
                return 27;
            return 28;
        }

        if (x <= 1073741824)
        {
            if (x <= 536870912)
                return 29;
            return 30;
        }

        return 31;
    }

    /// <summary>
    ///     Returns the square root of the specified <see cref="decimal" /> number.
    /// </summary>
    public static decimal Sqrt(decimal x, decimal epsilon = 0.0M)
    {
        if (x < 0)
            throw new OverflowException("Cannot calculate square root from a negative number.");

        decimal current = (decimal)Math.Sqrt((double)x), previous;

        do
        {
            previous = current;
            if (previous == 0.0M) return 0;
            current = (previous + x / previous) / 2;
        } while (Math.Abs(previous - current) > epsilon);

        return current;
    }

    /// <summary>
    /// Get decimals behind comma.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int GetDecimalCount(double value) => GetDecimalCount<double>(value);

    /// <summary>
    /// Get decimals behind comma.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int GetDecimalCount(decimal value) => GetDecimalCount<decimal>(value);

    /// <summary>
    /// Get decimals behind comma.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static int GetDecimalCount<T>(T value)
        where T : struct
    {
        int i = 0;

        if (value is decimal decNumber)
        {
            while (Math.Round(decNumber, i) != decNumber)
                i++;
        }
        else if (value is double dblNumber)
        {
            while (Math.Round(dblNumber, i) != dblNumber)
                i++;
        }
        else
        {
            throw new ArgumentException("Value can only be of type decimal or double");
        }

        return i;
    }
}