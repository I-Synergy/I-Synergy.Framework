using NumericRange = ISynergy.Framework.Core.Ranges.NumericRange;



namespace ISynergy.Framework.Mathematics.Vectors;

public static partial class Vector
{
    /// <summary>
    ///   Creates an interval vector (like NumPy's linspace function).
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.linspace function. For 
    ///   a similar method that accepts a step size instead of a number of steps, see
    ///   <see cref="Vector.Range(int, int)"/>.</para>
    /// </remarks>
    ///
    /// <seealso cref="Vector.Range(int, int)"/>
    ///
    public static double[] Interval(int a, int b)
    {
        if (a < b)
            return Interval(a, b, steps: b - a + 1);

        return Interval(a, b, steps: a - b + 1);
    }

    /// <summary>
    ///   Creates an interval vector (like NumPy's linspace function).
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.linspace function. For 
    ///   a similar method that accepts a step size instead of a number of steps, see
    ///   <see cref="Vector.Range(int, int)"/>.</para>
    /// </remarks>
    ///
    /// <seealso cref="Vector.Range(int, int)"/>
    ///
    public static double[] Interval(int a, int b, int steps, bool includeLast = true)
    {
        if (steps < 0)
            throw new ArgumentOutOfRangeException(nameof(steps), "The number of steps must be positive.");

        if (steps == 0)
            return [];

        if (steps == 1)
            return [a];

        if (a == b)
            return Vector.Create(size: steps, value: (double)a);

        double[] r = new double[steps];
        double length;
        if (includeLast)
        {
            length = steps - 1;
        }
        else
        {
            length = steps;
        }

        if (a > b)
        {
            var stepSize = (a - b) / length;
            for (uint i = 0; i < r.Length; i++)
                r[i] = a - i * stepSize;
        }
        else
        {
            var stepSize = (b - a) / length;
            for (uint i = 0; i < r.Length; i++)
                r[i] = a + i * stepSize;
        }

        if (includeLast)
            r[r.Length - 1] = b;

        return r;
    }

    /// <summary>
    ///   Creates an interval vector (like NumPy's linspace function).
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.linspace function. For 
    ///   a similar method that accepts a step size instead of a number of steps, see
    ///   <see cref="Vector.Range(int, int)"/>.</para>
    /// </remarks>
    ///
    /// <seealso cref="Vector.Range(int, int)"/>
    ///
    public static double[] Interval(double a, double b)
    {
        return Interval(a, b, steps: (int)Math.Ceiling(Math.Abs(a - b)));
    }

    /// <summary>
    ///   Creates an interval vector (like NumPy's linspace function).
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.linspace function. For 
    ///   a similar method that accepts a step size instead of a number of steps, see
    ///   <see cref="Vector.Range(int, int)"/>.</para>
    /// </remarks>
    ///
    /// <seealso cref="Vector.Range(int, int)"/>
    ///
    public static double[] Interval(double a, double b, int steps, bool includeLast = true)
    {
        if (steps < 0)
            throw new ArgumentOutOfRangeException(nameof(steps), "The number of steps must be positive.");

        if (steps == 0)
            return [];

        if (steps == 1)
            return [a];

        if (ExactComparisons.AreEqual(a ,b))
            return Vector.Create(size: steps, value: a);

        double[] r = new double[steps];
        double length;
        if (includeLast)
        {
            length = steps - 1;
        }
        else
        {
            length = steps;
        }

        if (a > b)
        {
            var stepSize = (a - b) / length;
            for (uint i = 0; i < r.Length; i++)
                r[i] = a - i * stepSize;
        }
        else
        {
            var stepSize = (b - a) / length;
            for (uint i = 0; i < r.Length; i++)
                r[i] = a + i * stepSize;
        }

        if (includeLast)
            r[r.Length - 1] = b;

        return r;
    }
    /// <summary>
    ///   Creates an interval vector (like NumPy's linspace function).
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.linspace function. For 
    ///   a similar method that accepts a step size instead of a number of steps, see
    ///   <see cref="Vector.Range(int, int)"/>.</para>
    /// </remarks>
    ///
    /// <seealso cref="Vector.Range(int, int)"/>
    ///
    public static double[] Interval(this NumericRange range, int steps)
    {
        return Interval(range.Min, range.Max, steps);
    }

    /// <summary>
    ///   Creates an interval vector using a fixed step size.
    /// </summary>
    ///
    /// <seealso cref="Vector.Range(double, double, double)"/>
    ///
    public static double[] Interval(double a, double b, double stepSize)
    {
        if (ExactComparisons.AreEqual(a ,b))
            return [a];

        double[] r;

        if (a > b)
        {
            int steps = (int)System.Math.Ceiling((a - b) / stepSize) + 1;
            r = new double[steps];
            for (uint i = 0; i < r.Length; i++)
                r[i] = a - i * stepSize;
            r[steps - 1] = b;
        }
        else
        {
            int steps = (int)System.Math.Ceiling((b - a) / stepSize) + 1;
            r = new double[steps];
            for (uint i = 0; i < r.Length; i++)
                r[i] = a + i * stepSize;
            r[steps - 1] = b;
        }

        return r;
    }

    /// <summary>
    ///   Creates an interval vector using a fixed step size.
    /// </summary>
    ///
    /// <seealso cref="Vector.Range(NumericRange, double)"/>
    ///
    public static double[] Interval(this NumericRange range, double stepSize)
    {
        return Interval(range.Min, range.Max, stepSize);
    }
}
