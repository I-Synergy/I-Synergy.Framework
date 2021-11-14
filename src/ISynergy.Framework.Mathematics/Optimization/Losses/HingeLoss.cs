namespace ISynergy.Framework.Mathematics.Optimization.Losses
{
    /// <summary>
    ///     Hinge loss.
    /// </summary>
    [Serializable]
    public struct HingeLoss : ILoss<double[]>,
        IDifferentiableLoss<bool, double, double>,
        IDifferentiableLoss<double, double, double>
    {
        private bool[][] expected;

        /// <summary>
        ///     Gets the expected outputs (the ground truth).
        /// </summary>
        public bool[][] Expected
        {
            get => expected;
            set => expected = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HingeLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public HingeLoss(double[][] expected)
        {
            this.expected = Classes.Decide(expected);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HingeLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public HingeLoss(double[] expected)
        {
            this.expected = Classes.Decide(Jagged.ColumnVector(expected));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HingeLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public HingeLoss(int[] expected)
        {
            if (Classes.IsMinusOnePlusOne(expected))
                expected = expected.ToZeroOne();

            this.expected = Jagged.OneHot<bool>(expected);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HingeLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public HingeLoss(bool[] expected)
        {
            this.expected = Jagged.ColumnVector(expected);
        }

        /// <summary>
        ///     Computes the loss between the expected values (ground truth)
        ///     and the given actual values that have been predicted.
        /// </summary>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <returns>
        ///     The loss value between the expected values and
        ///     the actual predicted values.
        /// </returns>
        public double Loss(double[][] actual)
        {
            double error = 0;
            for (var i = 0; i < Expected.Length; i++)
                for (var j = 0; j < Expected[i].Length; j++)
                    error += Loss(Expected[i][j], actual[i][j]);
            return error;
        }

        /// <summary>
        ///     Computes the loss between the expected values (ground truth)
        ///     and the given actual values that have been predicted.
        /// </summary>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <returns>
        ///     The loss value between the expected values and
        ///     the actual predicted values.
        /// </returns>
        public double Loss(double[] actual)
        {
            double error = 0;
            for (var i = 0; i < Expected.Length; i++)
                error += Loss(Expected[i][0], actual[i]);
            return error;
        }
        /// <summary>
        ///     Computes the derivative of the loss between the expected values (ground truth)
        ///     and the given actual values that have been predicted.
        /// </summary>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// <returns>
        ///     The loss value between the expected values and
        ///     the actual predicted values.
        /// </returns>
        public double Loss(bool expected, double actual)
        {
            if (expected)
            {
                if (actual > 1)
                    return 0;
                return 1 - actual;
            }

            if (actual < -1)
                return 0;
            return 1 + actual;
        }

        /// <summary>
        ///     Computes the derivative of the loss between the expected values (ground truth)
        ///     and the given actual values that have been predicted.
        /// </summary>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// <returns>
        ///     The loss value between the expected values and
        ///     the actual predicted values.
        /// </returns>
        public double Derivative(bool expected, double actual)
        {
            if (expected)
            {
                if (actual > 1)
                    return 0;
                return actual;
            }

            if (actual < -1)
                return 0;
            return actual;
        }

        /// <summary>
        ///     Computes the derivative of the loss between the expected values (ground truth)
        ///     and the given actual values that have been predicted.
        /// </summary>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// <returns>
        ///     The loss value between the expected values and
        ///     the actual predicted values.
        /// </returns>
        public double Loss(double expected, double actual)
        {
            return Loss(Classes.Decide(expected), actual);
        }

        /// <summary>
        ///     Computes the derivative of the loss between the expected values (ground truth)
        ///     and the given actual values that have been predicted.
        /// </summary>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// <returns>
        ///     The loss value between the expected values and
        ///     the actual predicted values.
        /// </returns>
        public double Derivative(double expected, double actual)
        {
            return Derivative(Classes.Decide(expected), actual);
        }
    }
}