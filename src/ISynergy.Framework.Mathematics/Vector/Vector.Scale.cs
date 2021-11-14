namespace ISynergy.Framework.Mathematics
{
    public static partial class Vector
    {
        /// <summary>
        ///   Converts a value from one scale to another scale.
        /// </summary>
        public static double Scale(this double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            if (fromMin == fromMax && fromMin == toMin && fromMin == toMax)
                return value;

            return ((toMax - toMin) * (value - fromMin) / (fromMax - fromMin) + toMin);
        }

        /// <summary>
        /// Converts values from one scale to another scale.
        /// </summary>
        /// <param name="fromMin"></param>
        /// <param name="fromMax"></param>
        /// <param name="toMin"></param>
        /// <param name="toMax"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double[][] Scale(double[] fromMin, double[] fromMax, double toMin, double toMax, double[][] x)
        {
            int rows = x.Length;
            int cols = fromMin.Length;

            double[][] result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = (toMax - toMin) * (x[i][j] - fromMin[j]) / (fromMax[j] - fromMin[j]) + toMin;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts values from one scale to another scale.
        /// </summary>
        public static double[] Scale(this double[] values, double fromMin, double fromMax, double toMin, double toMax, double[] result)
        {
            if (fromMin == fromMax && fromMin == toMin && fromMin == toMax)
            {
                for (var i = 0; i < values.Length; i++)
                    result[i] = values[i];

                return result;
            }

            for (var i = 0; i < values.Length; i++)
                result[i] = ((toMax - toMin) * (values[i] - fromMin) / (fromMax - fromMin) + toMin);

            return result;
        }

        /// <summary>
        ///   Converts values from one scale to another scale.
        /// </summary>
        public static double Scale(this double value, NumericRange fromRange, NumericRange toRange)
        {
            return Scale(value, fromRange.Min, fromRange.Max, toRange.Min, toRange.Max);
        }

        /// <summary>
        ///   Converts values from one scale to another scale.
        /// </summary>
        public static double[] Scale(this double[] values, double fromMin, double fromMax, double toMin, double toMax)
        {
            return Scale(values, fromMin, fromMax, toMin, toMax, new double[values.Length]);
        }

        /// <summary>
        ///   Converts values from one scale to another scale.
        /// </summary>
        public static double[] Scale(this double[] values, double toMin, double toMax)
        {
            double[] result = new double[values.Length];
            values.GetRange(out double fromMin, out double fromMax);
            return Scale(values, fromMin, fromMax, toMin, toMax, result);
        }

        /// <summary>
        ///   Converts values from one scale to another scale.
        /// </summary>
        public static double[] Scale(this double[] values, NumericRange fromRange, NumericRange toRange, double[] result)
        {
            return Scale(values, fromRange.Min, fromRange.Max, toRange.Min, toRange.Max, result);
        }
        /// <summary>
        ///   Converts values from one scale to another scale.
        /// </summary>
        public static double[] Scale(this double[] values, NumericRange fromRange, NumericRange toRange)
        {
            return Scale(values, fromRange.Min, fromRange.Max, toRange.Min, toRange.Max, new double[values.Length]);
        }

        /// <summary>
        ///   Converts values from one scale to another scale.
        /// </summary>
        public static double[] Scale(this double[] values, NumericRange toRange, double[] result)
        {
            values.GetRange(out double fromMin, out double fromMax);
            return Scale(values, fromMin, fromMax, toRange.Min, toRange.Max, result);
        }

        /// <summary>
        ///   Converts values from one scale to another scale.
        /// </summary>
        public static double[] Scale(this double[] values, double fromMin, double fromMax, int toMin, int toMax, double[] result)
        {
            if (fromMin == fromMax && fromMin == toMin && fromMin == toMax)
            {
                for (var i = 0; i < values.Length; i++)
                    result[i] = values[i];
                return result;
            }

            for (var i = 0; i < values.Length; i++)
                result[i] = ((toMax - toMin) * (values[i] - fromMin) / (fromMax - fromMin) + toMin);

            return result;
        }

        /// <summary>
        ///   Converts values from one scale to another scale.
        /// </summary>
        public static double[] Scale(this double[] values, double fromMin, double fromMax, int toMin, int toMax)
        {
            return Scale(values, fromMin, fromMax, toMin, toMax, new double[values.Length]);
        }

        /// <summary>
        ///   Converts values from one scale to another scale.
        /// </summary>
        public static double[] Scale(this double[] values, int toMin, int toMax)
        {
            double[] result = new double[values.Length];
            values.GetRange(out double fromMin, out double fromMax);
            return Scale(values, fromMin, fromMax, toMin, toMax, result);
        }

        /// <summary>
        ///   Converts values from one scale to another scale.
        /// </summary>
        public static double[] Scale(this double[] values, double toMin, double toMax, double[] result)
        {
            values.GetRange(out double fromMin, out double fromMax);
            return Scale(values, fromMin, fromMax, toMin, toMax, result);
        }
    }
}
