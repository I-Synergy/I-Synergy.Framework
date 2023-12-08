namespace ISynergy.Framework.Mathematics;

using System;
using System.Collections.Generic;
using NumericRange = ISynergy.Framework.Core.Ranges.NumericRange;

public static partial class Vector
{
    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="n">The exclusive upper bound of the range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the Interval methods.</para>  
    /// </remarks>
    public static int[] Range(int n)
    {
        int[] r = new int[(int)n];
        for (int i = 0; i < r.Length; i++)
            r[i] = (int)i;
        return r;
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static int[] Range(int a, int b, int stepSize)
    {
        if (a == b)
            return new int[] { };

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        int[] r;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)Math.Ceiling(((double)(b - a) / (double)stepSize));

            r = new int[steps];
            for (uint i = 0; i < r.Length; i++)
                r[i] = (int)(a + i * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)Math.Ceiling(((double)(a - b) / (double)stepSize));
                r = new int[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (int)(a - i * stepSize);
            }
            else
            {
                uint steps = (uint)Math.Ceiling(((double)(b - a) / (double)stepSize));
                r = new int[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (int)(a + i * stepSize);
            }
        }

        if (a < b)
        {
            if (r[r.Length - 1] > b)
                r[r.Length - 1] = b;
        }
        else
        {
            if (r[r.Length - 1] > a)
                r[r.Length - 1] = a;
        }

        return r;
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="n">The exclusive upper bound of the range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static double[] Range(double n)
    {
        double[] r = new double[(int)n];
        for (var i = 0; i < r.Length; i++)
            r[i] = (double)i;
        return r;
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    ///
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static double[] Range(double a, double b)
    {
        if (a == b)
            return new double[] { };

        double[] r;

        if (b > a)
        {
            r = new double[(int)(b - a)];
            for (var i = 0; i < r.Length; i++)
                r[i] = (double)(a++);
        }
        else
        {
            r = new double[(int)(a - b)];
            for (var i = 0; i < r.Length; i++)
                r[i] = (double)(a--);
        }

        return r;
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    ///
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static int[] Range(int a, int b)
    {
        if (a == b)
            return new int[] { };

        int[] r;

        if (b > a)
        {
            r = new int[(b - a)];
            for (var i = 0; i < r.Length; i++)
                r[i] = a++;
        }
        else
        {
            r = new int[(a - b)];
            for (var i = 0; i < r.Length; i++)
                r[i] = a--;
        }

        return r;
    }

    /// <summary>
    ///   Enumerates through a range (like Python's xrange function).
    /// </summary>
    /// 
    /// <param name="n">The exclusive upper bound of the range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static IEnumerable<double> EnumerableRange(double n)
    {
        for (double i = 0; i < n; i++)
            yield return (double)i;
    }

    /// <summary>
    ///   Enumerates through a range (like Python's xrange).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static IEnumerable<double> EnumerableRange(double a, double b)
    {
        if (a == b)
            yield break;

        if (b > a)
        {
            int n = (int)(b - a);
            for (var i = 0; i < n; i++)
                yield return (double)(a++);
        }
        else
        {
            int n = (int)(a - b);
            for (var i = 0; i < n; i++)
                yield return (double)(a--);
        }
    }
    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static double[] Range(double a, double b, int stepSize)
    {
        if (a == b)
            return new double[] { };

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double[] r;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));

            r = new double[steps];
            for (uint i = 0; i < r.Length; i++)
                r[i] = (double)(a + i * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a - i * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a + i * stepSize);
            }
        }

        if (a < b)
        {
            if (r[r.Length - 1] > b)
                r[r.Length - 1] = b;
        }
        else
        {
            if (r[r.Length - 1] > a)
                r[r.Length - 1] = a;
        }

        return r;
    }

    /// <summary>
    ///   Enumerates through a range (like Python's xrange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static IEnumerable<double> EnumerableRange(double a, double b, int stepSize)
    {
        if (a == b)
            yield break;

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double last;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
            for (uint i = 0; i < steps; i++)
                yield return (double)(a + i * stepSize);
            last = (double)(a + steps * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a - i * stepSize);
                last = (double)(a - steps * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a + i * stepSize);
                last = (double)(a + steps * stepSize);
            }
        }

        if (a < b)
        {
            yield return last > b ? b : last;
        }
        else
        {
            yield return last > a ? a : last;
        }
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static double[] Range(double a, double b, double stepSize)
    {
        if (a == b)
            return new double[] { };

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double[] r;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));

            r = new double[steps];
            for (uint i = 0; i < r.Length; i++)
                r[i] = (double)(a + i * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a - i * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a + i * stepSize);
            }
        }

        if (a < b)
        {
            if (r[r.Length - 1] > b)
                r[r.Length - 1] = b;
        }
        else
        {
            if (r[r.Length - 1] > a)
                r[r.Length - 1] = a;
        }

        return r;
    }

    /// <summary>
    ///   Enumerates through a range (like Python's xrange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static IEnumerable<double> EnumerableRange(double a, double b, double stepSize)
    {
        if (a == b)
            yield break;

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double last;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
            for (uint i = 0; i < steps; i++)
                yield return (double)(a + i * stepSize);
            last = (double)(a + steps * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a - i * stepSize);
                last = (double)(a - steps * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a + i * stepSize);
                last = (double)(a + steps * stepSize);
            }
        }

        if (a < b)
        {
            yield return last > b ? b : last;
        }
        else
        {
            yield return last > a ? a : last;
        }
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static double[] Range(double a, double b, short stepSize)
    {
        if (a == b)
            return new double[] { };

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double[] r;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));

            r = new double[steps];
            for (uint i = 0; i < r.Length; i++)
                r[i] = (double)(a + i * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a - i * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a + i * stepSize);
            }
        }

        if (a < b)
        {
            if (r[r.Length - 1] > b)
                r[r.Length - 1] = b;
        }
        else
        {
            if (r[r.Length - 1] > a)
                r[r.Length - 1] = a;
        }

        return r;
    }

    /// <summary>
    ///   Enumerates through a range (like Python's xrange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static IEnumerable<double> EnumerableRange(double a, double b, short stepSize)
    {
        if (a == b)
            yield break;

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double last;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
            for (uint i = 0; i < steps; i++)
                yield return (double)(a + i * stepSize);
            last = (double)(a + steps * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a - i * stepSize);
                last = (double)(a - steps * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a + i * stepSize);
                last = (double)(a + steps * stepSize);
            }
        }

        if (a < b)
        {
            yield return last > b ? b : last;
        }
        else
        {
            yield return last > a ? a : last;
        }
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static double[] Range(double a, double b, byte stepSize)
    {
        if (a == b)
            return new double[] { };

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double[] r;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));

            r = new double[steps];
            for (uint i = 0; i < r.Length; i++)
                r[i] = (double)(a + i * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a - i * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a + i * stepSize);
            }
        }

        if (a < b)
        {
            if (r[r.Length - 1] > b)
                r[r.Length - 1] = b;
        }
        else
        {
            if (r[r.Length - 1] > a)
                r[r.Length - 1] = a;
        }

        return r;
    }

    /// <summary>
    ///   Enumerates through a range (like Python's xrange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static IEnumerable<double> EnumerableRange(double a, double b, byte stepSize)
    {
        if (a == b)
            yield break;

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double last;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
            for (uint i = 0; i < steps; i++)
                yield return (double)(a + i * stepSize);
            last = (double)(a + steps * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a - i * stepSize);
                last = (double)(a - steps * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a + i * stepSize);
                last = (double)(a + steps * stepSize);
            }
        }

        if (a < b)
        {
            yield return last > b ? b : last;
        }
        else
        {
            yield return last > a ? a : last;
        }
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static double[] Range(double a, double b, sbyte stepSize)
    {
        if (a == b)
            return new double[] { };

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double[] r;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));

            r = new double[steps];
            for (uint i = 0; i < r.Length; i++)
                r[i] = (double)(a + i * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a - i * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a + i * stepSize);
            }
        }

        if (a < b)
        {
            if (r[r.Length - 1] > b)
                r[r.Length - 1] = b;
        }
        else
        {
            if (r[r.Length - 1] > a)
                r[r.Length - 1] = a;
        }

        return r;
    }

    /// <summary>
    ///   Enumerates through a range (like Python's xrange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static IEnumerable<double> EnumerableRange(double a, double b, sbyte stepSize)
    {
        if (a == b)
            yield break;

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double last;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
            for (uint i = 0; i < steps; i++)
                yield return (double)(a + i * stepSize);
            last = (double)(a + steps * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a - i * stepSize);
                last = (double)(a - steps * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a + i * stepSize);
                last = (double)(a + steps * stepSize);
            }
        }

        if (a < b)
        {
            yield return last > b ? b : last;
        }
        else
        {
            yield return last > a ? a : last;
        }
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static double[] Range(double a, double b, long stepSize)
    {
        if (a == b)
            return new double[] { };

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double[] r;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));

            r = new double[steps];
            for (uint i = 0; i < r.Length; i++)
                r[i] = (double)(a + i * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a - i * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a + i * stepSize);
            }
        }

        if (a < b)
        {
            if (r[r.Length - 1] > b)
                r[r.Length - 1] = b;
        }
        else
        {
            if (r[r.Length - 1] > a)
                r[r.Length - 1] = a;
        }

        return r;
    }

    /// <summary>
    ///   Enumerates through a range (like Python's xrange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static IEnumerable<double> EnumerableRange(double a, double b, long stepSize)
    {
        if (a == b)
            yield break;

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double last;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
            for (uint i = 0; i < steps; i++)
                yield return (double)(a + i * stepSize);
            last = (double)(a + steps * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a - i * stepSize);
                last = (double)(a - steps * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a + i * stepSize);
                last = (double)(a + steps * stepSize);
            }
        }

        if (a < b)
        {
            yield return last > b ? b : last;
        }
        else
        {
            yield return last > a ? a : last;
        }
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static double[] Range(double a, double b, ulong stepSize)
    {
        if (a == b)
            return new double[] { };

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double[] r;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));

            r = new double[steps];
            for (uint i = 0; i < r.Length; i++)
                r[i] = (double)(a + i * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a - i * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a + i * stepSize);
            }
        }

        if (a < b)
        {
            if (r[r.Length - 1] > b)
                r[r.Length - 1] = b;
        }
        else
        {
            if (r[r.Length - 1] > a)
                r[r.Length - 1] = a;
        }

        return r;
    }

    /// <summary>
    ///   Enumerates through a range (like Python's xrange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static IEnumerable<double> EnumerableRange(double a, double b, ulong stepSize)
    {
        if (a == b)
            yield break;

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double last;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
            for (uint i = 0; i < steps; i++)
                yield return (double)(a + i * stepSize);
            last = (double)(a + steps * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a - i * stepSize);
                last = (double)(a - steps * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a + i * stepSize);
                last = (double)(a + steps * stepSize);
            }
        }

        if (a < b)
        {
            yield return last > b ? b : last;
        }
        else
        {
            yield return last > a ? a : last;
        }
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static double[] Range(double a, double b, ushort stepSize)
    {
        if (a == b)
            return new double[] { };

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double[] r;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));

            r = new double[steps];
            for (uint i = 0; i < r.Length; i++)
                r[i] = (double)(a + i * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a - i * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize));
                r = new double[steps];
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a + i * stepSize);
            }
        }

        if (a < b)
        {
            if (r[r.Length - 1] > b)
                r[r.Length - 1] = b;
        }
        else
        {
            if (r[r.Length - 1] > a)
                r[r.Length - 1] = a;
        }

        return r;
    }

    /// <summary>
    ///   Enumerates through a range (like Python's xrange function).
    /// </summary>
    /// 
    /// <param name="a">The inclusive lower bound of the range.</param>
    /// <param name="b">The exclusive upper bound of the range.</param>
    /// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static IEnumerable<double> EnumerableRange(double a, double b, ushort stepSize)
    {
        if (a == b)
            yield break;

        if (stepSize == 0)
            throw new ArgumentOutOfRangeException("stepSize", "stepSize must be different from zero.");

        double last;

        if (a < b)
        {
            if (stepSize < 0)
                throw new ArgumentOutOfRangeException("stepSize", "If a < b, stepSize must be positive.");

            uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
            for (uint i = 0; i < steps; i++)
                yield return (double)(a + i * stepSize);
            last = (double)(a + steps * stepSize);
        }
        else
        {
            if (stepSize > 0)
            {
                uint steps = (uint)System.Math.Ceiling(((double)(a - b) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a - i * stepSize);
                last = (double)(a - steps * stepSize);
            }
            else
            {
                uint steps = (uint)System.Math.Ceiling(((double)(b - a) / (double)stepSize)) - 1;
                for (uint i = 0; i < steps; i++)
                    yield return (double)(a + i * stepSize);
                last = (double)(a + steps * stepSize);
            }
        }

        if (a < b)
        {
            yield return last > b ? b : last;
        }
        else
        {
            yield return last > a ? a : last;
        }
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="range">The range from where values should be created.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static double[] Range(this NumericRange range)
    {
        return Range(range.Min, range.Max);
    }

    /// <summary>
    ///   Creates a range vector (like NumPy's arange function).
    /// </summary>
    /// 
    /// <param name="range">The range from where values should be created.</param>
		/// <param name="stepSize">The step size to be taken between elements. 
    ///   This parameter can be negative to create a decreasing range.</param>
		///
    /// <remarks>
    /// <para>
    ///   The Range methods should be equivalent to NumPy's np.arange method, with one
    ///   single difference: when the intervals are inverted (i.e. a > b) and the step
    ///   size is negative, the framework still iterates over the range backwards, as 
    ///   if the step was negative.</para>
    /// <para>
    ///   This function never includes the upper bound of the range. For methods
    ///   that include it, please see the <see cref="Interval(int, int)"/> methods.</para>  
    /// </remarks>
    ///
    /// <seealso cref="Interval(int, int)"/>
    ///
    public static double[] Range(this NumericRange range, double stepSize)
    {
        return Range(range.Min, range.Max, stepSize);
    }

    /// <summary>
    ///   Creates a index vector.
    /// </summary>
    /// 
    public static double[] Indices(int from, int to)
    {
        if (from > to)
            return Vector.Range(Convert.ToDouble(from - 1), Convert.ToDouble(to - 1));
        return Vector.Range(Convert.ToDouble(from), Convert.ToDouble(to));
    }
}