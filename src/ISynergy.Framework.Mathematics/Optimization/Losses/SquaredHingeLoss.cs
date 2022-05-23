using System;

namespace ISynergy.Framework.Mathematics.Optimization.Losses
{
    /// <summary>
    ///     Squared Hinge loss.
    /// </summary>
    [Serializable]
    public struct SquaredHingeLoss : ILoss<double[]>,
        IDifferentiableLoss<bool, double, double>,
        IDifferentiableLoss<double, double, double>
    {
        private HingeLoss hinge;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SquaredHingeLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public SquaredHingeLoss(double[][] expected)
        {
            hinge = new HingeLoss(expected);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SquaredHingeLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public SquaredHingeLoss(double[] expected)
        {
            hinge = new HingeLoss(expected);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SquaredHingeLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public SquaredHingeLoss(int[] expected)
        {
            hinge = new HingeLoss(expected);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SquaredHingeLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public SquaredHingeLoss(bool[] expected)
        {
            hinge = new HingeLoss(expected);
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
            for (var i = 0; i < hinge.Expected.Length; i++)
                error += Loss(hinge.Expected[i][0], actual[i]);
            return error;
        }

        /// <summary>
        ///     Computes the derivative of the loss between the expected values (ground truth)
        ///     and the given actual values that have been predicted.
        /// </summary>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// <param name="actual">The actual values that have been predicted.</param>
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
                var d = 1 - actual;
                return 0.5 * d * d;
            }
            else
            {
                if (actual < -1)
                    return 0;
                var d = 1 + actual;
                return 0.5 * d * d;
            }
        }

        /// <summary>
        ///     Computes the derivative of the loss between the expected values (ground truth)
        ///     and the given actual values that have been predicted.
        /// </summary>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// <param name="actual">The actual values that have been predicted.</param>
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
                return actual * (1 - actual);
            }

            if (actual < -1)
                return 0;
            return -actual * (1 + actual);
        }

        /// <summary>
        ///     Computes the derivative of the loss between the expected values (ground truth)
        ///     and the given actual values that have been predicted.
        /// </summary>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <returns>
        ///     The loss value between the expected values and
        ///     the actual predicted values.
        /// </returns>
        public double Loss(double expected, double actual)
        {
            // TODO: Use multiplication instead of conditionals
            if (expected > 0)
            {
                if (actual > 1)
                    return 0;
                var d = 1 - actual;
                return 0.5 * d * d;
            }
            else
            {
                if (actual < -1)
                    return 0;
                var d = 1 + actual;
                return 0.5 * d * d;
            }
        }

        /// <summary>
        ///     Computes the derivative of the loss between the expected values (ground truth)
        ///     and the given actual values that have been predicted.
        /// </summary>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <returns>
        ///     The loss value between the expected values and
        ///     the actual predicted values.
        /// </returns>
        public double Derivative(double expected, double actual)
        {
            // TODO: Use multiplication instead of conditionals
            if (expected > 0)
            {
                if (actual > 1)
                    return 0;
                return actual * (1 - actual);
            }

            if (actual < -1)
                return 0;
            return -actual * (1 + actual);
        }
    }
}