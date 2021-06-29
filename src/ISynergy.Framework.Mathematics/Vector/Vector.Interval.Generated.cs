namespace ISynergy.Framework.Mathematics
{
    using System;
    using ISynergy.Framework.Core.Ranges;
    using ISynergy.Framework.Mathematics;
    using Range = ISynergy.Framework.Core.Ranges.Range;

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
        public static int[] Interval(int a, int b)
        {
			if (a < b)
				return Interval(a, b, steps: (int)(b - a) + 1);
			return Interval(a, b, steps: (int)(a - b) + 1);
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
        public static int[] Interval(int a, int b, int steps, bool includeLast = true)
        {
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps", "The number of steps must be positive.");

			if (steps == 0)
				return new int[] { };

			if (steps == 1)
				return new int[] { a };

            if (a == b)
                return Vector.Create(size: steps, value: a);
			
            int[] r = new int[steps];
			double length;
			if (includeLast)
			{
				length = ((int)(steps - 1)); 
			}
			else
			{
				length = ((int)(steps));
			}

            if (a > b)
            {
                var stepSize = (double)((a - b) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (int)(a - i * stepSize);
            }
            else
            {
                var stepSize = (double)((b - a) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (int)(a + i * stepSize);
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
        public static float[] Interval(float a, float b)
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
        public static float[] Interval(float a, float b, int steps, bool includeLast = true)
        {
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps", "The number of steps must be positive.");

			if (steps == 0)
				return new float[] { };

			if (steps == 1)
				return new float[] { a };

            if (a == b)
                return Vector.Create(size: steps, value: a);
			
            float[] r = new float[steps];
			double length;
			if (includeLast)
			{
				length = ((float)(steps - 1)); 
			}
			else
			{
				length = ((float)(steps));
			}

            if (a > b)
            {
                var stepSize = (double)((a - b) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (float)(a - i * stepSize);
            }
            else
            {
                var stepSize = (double)((b - a) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (float)(a + i * stepSize);
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
				throw new ArgumentOutOfRangeException("steps", "The number of steps must be positive.");

			if (steps == 0)
				return new double[] { };

			if (steps == 1)
				return new double[] { a };

            if (a == b)
                return Vector.Create(size: steps, value: a);
			
            double[] r = new double[steps];
			double length;
			if (includeLast)
			{
				length = ((double)(steps - 1)); 
			}
			else
			{
				length = ((double)(steps));
			}

            if (a > b)
            {
                var stepSize = (double)((a - b) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a - i * stepSize);
            }
            else
            {
                var stepSize = (double)((b - a) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (double)(a + i * stepSize);
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
        public static short[] Interval(short a, short b)
        {
			if (a < b)
				return Interval(a, b, steps: (int)(b - a) + 1);
			return Interval(a, b, steps: (int)(a - b) + 1);
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
        public static short[] Interval(short a, short b, int steps, bool includeLast = true)
        {
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps", "The number of steps must be positive.");

			if (steps == 0)
				return new short[] { };

			if (steps == 1)
				return new short[] { a };

            if (a == b)
                return Vector.Create(size: steps, value: a);
			
            short[] r = new short[steps];
			double length;
			if (includeLast)
			{
				length = ((short)(steps - 1)); 
			}
			else
			{
				length = ((short)(steps));
			}

            if (a > b)
            {
                var stepSize = (double)((a - b) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (short)(a - i * stepSize);
            }
            else
            {
                var stepSize = (double)((b - a) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (short)(a + i * stepSize);
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
        public static byte[] Interval(byte a, byte b)
        {
			if (a < b)
				return Interval(a, b, steps: (int)(b - a) + 1);
			return Interval(a, b, steps: (int)(a - b) + 1);
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
        public static byte[] Interval(byte a, byte b, int steps, bool includeLast = true)
        {
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps", "The number of steps must be positive.");

			if (steps == 0)
				return new byte[] { };

			if (steps == 1)
				return new byte[] { a };

            if (a == b)
                return Vector.Create(size: steps, value: a);
			
            byte[] r = new byte[steps];
			double length;
			if (includeLast)
			{
				length = ((byte)(steps - 1)); 
			}
			else
			{
				length = ((byte)(steps));
			}

            if (a > b)
            {
                var stepSize = (double)((a - b) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (byte)(a - i * stepSize);
            }
            else
            {
                var stepSize = (double)((b - a) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (byte)(a + i * stepSize);
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
        public static sbyte[] Interval(sbyte a, sbyte b)
        {
			if (a < b)
				return Interval(a, b, steps: (int)(b - a) + 1);
			return Interval(a, b, steps: (int)(a - b) + 1);
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
        public static sbyte[] Interval(sbyte a, sbyte b, int steps, bool includeLast = true)
        {
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps", "The number of steps must be positive.");

			if (steps == 0)
				return new sbyte[] { };

			if (steps == 1)
				return new sbyte[] { a };

            if (a == b)
                return Vector.Create(size: steps, value: a);
			
            sbyte[] r = new sbyte[steps];
			double length;
			if (includeLast)
			{
				length = ((sbyte)(steps - 1)); 
			}
			else
			{
				length = ((sbyte)(steps));
			}

            if (a > b)
            {
                var stepSize = (double)((a - b) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (sbyte)(a - i * stepSize);
            }
            else
            {
                var stepSize = (double)((b - a) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (sbyte)(a + i * stepSize);
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
        public static long[] Interval(long a, long b)
        {
			if (a < b)
				return Interval(a, b, steps: (int)(b - a) + 1);
			return Interval(a, b, steps: (int)(a - b) + 1);
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
        public static long[] Interval(long a, long b, int steps, bool includeLast = true)
        {
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps", "The number of steps must be positive.");

			if (steps == 0)
				return new long[] { };

			if (steps == 1)
				return new long[] { a };

            if (a == b)
                return Vector.Create(size: steps, value: a);
			
            long[] r = new long[steps];
			double length;
			if (includeLast)
			{
				length = ((long)(steps - 1)); 
			}
			else
			{
				length = ((long)(steps));
			}

            if (a > b)
            {
                var stepSize = (double)((a - b) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (long)(a - i * stepSize);
            }
            else
            {
                var stepSize = (double)((b - a) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (long)(a + i * stepSize);
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
        public static decimal[] Interval(decimal a, decimal b)
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
        public static decimal[] Interval(decimal a, decimal b, int steps, bool includeLast = true)
        {
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps", "The number of steps must be positive.");

			if (steps == 0)
				return new decimal[] { };

			if (steps == 1)
				return new decimal[] { a };

            if (a == b)
                return Vector.Create(size: steps, value: a);
			
            decimal[] r = new decimal[steps];
			decimal length;
			if (includeLast)
			{
				length = ((decimal)(steps - 1)); 
			}
			else
			{
				length = ((decimal)(steps));
			}

            if (a > b)
            {
                var stepSize = (decimal)((a - b) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (decimal)(a - i * stepSize);
            }
            else
            {
                var stepSize = (decimal)((b - a) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (decimal)(a + i * stepSize);
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
        public static ulong[] Interval(ulong a, ulong b)
        {
			if (a < b)
				return Interval(a, b, steps: (int)(b - a) + 1);
			return Interval(a, b, steps: (int)(a - b) + 1);
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
        public static ulong[] Interval(ulong a, ulong b, int steps, bool includeLast = true)
        {
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps", "The number of steps must be positive.");

			if (steps == 0)
				return new ulong[] { };

			if (steps == 1)
				return new ulong[] { a };

            if (a == b)
                return Vector.Create(size: steps, value: a);
			
            ulong[] r = new ulong[steps];
			double length;
			if (includeLast)
			{
				length = ((ulong)(steps - 1)); 
			}
			else
			{
				length = ((ulong)(steps));
			}

            if (a > b)
            {
                var stepSize = (double)((a - b) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (ulong)(a - i * stepSize);
            }
            else
            {
                var stepSize = (double)((b - a) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (ulong)(a + i * stepSize);
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
        public static ushort[] Interval(ushort a, ushort b)
        {
			if (a < b)
				return Interval(a, b, steps: (int)(b - a) + 1);
			return Interval(a, b, steps: (int)(a - b) + 1);
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
        public static ushort[] Interval(ushort a, ushort b, int steps, bool includeLast = true)
        {
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps", "The number of steps must be positive.");

			if (steps == 0)
				return new ushort[] { };

			if (steps == 1)
				return new ushort[] { a };

            if (a == b)
                return Vector.Create(size: steps, value: a);
			
            ushort[] r = new ushort[steps];
			double length;
			if (includeLast)
			{
				length = ((ushort)(steps - 1)); 
			}
			else
			{
				length = ((ushort)(steps));
			}

            if (a > b)
            {
                var stepSize = (double)((a - b) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (ushort)(a - i * stepSize);
            }
            else
            {
                var stepSize = (double)((b - a) / length);
                for (uint i = 0; i < r.Length; i++)
                    r[i] = (ushort)(a + i * stepSize);
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
        public static double[] Interval(this DoubleRange range, int steps)
        {
            return Interval(range.Min, range.Max, steps);
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
        public static float[] Interval(this Range range, int steps)
        {
            return Interval(range.Min, range.Max, steps);
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
        public static byte[] Interval(this ByteRange range, int steps)
        {
            return Interval(range.Min, range.Max, steps);
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
        public static int[] Interval(this IntRange range, int steps)
        {
            return Interval(range.Min, range.Max, steps);
        }
    }
}