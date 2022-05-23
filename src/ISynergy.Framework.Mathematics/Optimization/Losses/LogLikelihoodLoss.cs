using System;

namespace ISynergy.Framework.Mathematics.Optimization.Losses
{
    /// <summary>
    ///     Negative log-likelihood loss.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The log-likelihood loss can be used to measure the performance of unsupervised
    ///         model fitting algorithms. It simply computes the sum of all log-likelihood values
    ///         produced by the model.
    ///     </para>
    ///     <para>
    ///         If you would like to measure the performance of a supervised classification model
    ///         based on their probability predictions, please refer to the <see cref="BinaryCrossEntropyLoss" />
    ///         and <see cref="CategoryCrossEntropyLoss" /> for binary and multi-class decision problems,
    ///         respectively.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <para>
    ///         The following example shows how to learn an one-class SVM
    ///         and measure its performance using the log-likelihood loss class.
    ///     </para>
    /// </example>
    /// <seealso cref="BinaryCrossEntropyLoss" />
    /// <seealso cref="CategoryCrossEntropyLoss" />
    [Serializable]
    public class LogLikelihoodLoss : ILoss<double[][]>, ILoss<double[]>
    {
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
            for (var i = 0; i < actual.Length; i++)
                for (var j = 0; j < actual[i].Length; j++)
                    error += actual[i][j];
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
            for (var i = 0; i < actual.Length; i++)
                error += actual[i];
            return error;
        }
    }
}