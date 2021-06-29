using ISynergy.Framework.Mathematics.Statistics;
using System;

namespace ISynergy.Framework.Mathematics.Optimization.Losses
{
    /// <summary>
    ///     R² (r-squared) loss.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The coefficient of determination is used in the context of statistical models
    ///         whose main purpose is the prediction of future outcomes on the basis of other
    ///         related information. It is the proportion of variability in a data set that
    ///         is accounted for by the statistical model. It provides a measure of how well
    ///         future outcomes are likely to be predicted by the model.
    ///     </para>
    ///     <para>
    ///         The R² coefficient of determination is a statistical measure of how well the
    ///         regression line approximates the real data points. An R² of 1.0 indicates
    ///         that the regression line perfectly fits the data.
    ///     </para>
    ///     <para>
    ///         References:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>
    ///                     <a href="https://en.wikipedia.org/wiki/Coefficient_of_determination">
    ///                         Wikipedia contributors. Coefficient of determination. Wikipedia, The Free Encyclopedia.
    ///                         September 6, 2017, 19:48 UTC. Available at:
    ///                         https://en.wikipedia.org/wiki/Coefficient_of_determination.
    ///                     </a>
    ///                 </description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <para>
    ///         This example shows how to fit a multiple linear regression model and compute
    ///         adjusted and non-adjusted versions of the R² coefficient of determination at
    ///         the end:
    ///     </para>
    ///     <code source="tests\ISynergy.Framework.Mathematics.Tests.Statistics\Models\Regression\MultipleLinearRegressionTest.cs"
    ///         region="doc_learn_2" />
    /// </example>
    /// <seealso cref="SquareLoss" />
    [Serializable]
    public class RSquaredLoss : LossBase<double[][], double[][], double[]>, ILoss<double[]>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RSquaredLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// <param name="numberOfInputs">The number if variables being fit.</param>
        public RSquaredLoss(int numberOfInputs, double[] expected)
        {
            Expected = Jagged.ColumnVector(expected);
            NumberOfInputs = numberOfInputs;
            Weights = Vector.Ones(expected.Length);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RSquaredLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// <param name="numberOfInputs">The number if variables being fit.</param>
        public RSquaredLoss(int numberOfInputs, double[][] expected)
        {
            Expected = expected;
            NumberOfInputs = numberOfInputs;
            Weights = Vector.Ones(expected.Length);
        }

        /// <summary>
        ///     Gets or sets the number of variables being fit in the problem.
        /// </summary>
        public int NumberOfInputs { get; set; }

        /// <summary>
        ///     Gets whether the adjusted version of the R²
        ///     measure should be computed instead.
        /// </summary>
        public bool Adjust { get; set; }

        /// <summary>
        ///     Gets or sets the weights associated with each input-output pair.
        /// </summary>
        public double[] Weights { get; set; }

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
            // R-squared = 100 * SS(regression) / SS(total)

            var p = NumberOfInputs;
            var n = actual.Length;
            var SSe = 0.0;
            var SSt = 0.0;

            // Calculate output mean
            double avg = Expected.WeightedMean(Weights).Mean();

            // Calculate SSe and SSt
            for (var i = 0; i < Expected.Length; i++)
            {
                var w = Weights[i];

                var d = Expected[i][0] - actual[i];
                SSe += w * d * d;

                d = Expected[i][0] - avg;
                SSt += w * d * d;
            }

            // Calculate R-Squared
            var r2 = 1 - (Math.Abs(SSt - SSe) / n > 1e-16 ? SSe / SSt : 0);

            if (!Adjust)
                // Return ordinary R-Squared
                return r2;

            if (r2 == 1)
                return 1;

            if (n - p == 1.0)
                return double.NaN;

            // Return adjusted R-Squared
            return 1.0 - (1.0 - r2) * ((n - 1.0) / (n - p - 1.0));
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
        public override double[] Loss(double[][] actual)
        {
            var N = actual.Length;
            var M = actual.Columns();
            var P = NumberOfInputs;
            var SSe = new double[M];
            var SSt = new double[M];
            var r2 = new double[M];

            // For each output variable
            double[] avg = Expected.WeightedMean(Weights);

            // Calculate SSe and SSt
            for (var i = 0; i < N; i++)
                for (var c = 0; c < M; c++)
                {
                    var w = Weights[i];

                    double d;
                    d = Expected[i][c] - actual[i][c];
                    SSe[c] += w * d * d;

                    d = Expected[i][c] - avg[c];
                    SSt[c] += w * d * d;
                }

            // Calculate R-Squared
            for (var c = 0; c < M; c++)
                r2[c] = 1 - (Math.Abs(SSt[c] - SSe[c]) / N > 1e-16 ? SSe[c] / SSt[c] : 0);

            if (Adjust)
                // Return adjusted R-Squared
                for (var c = 0; c < M; c++)
                {
                    if (r2[c] == 1.0)
                        continue;

                    if (N == P + 1)
                        r2[c] = double.NaN;
                    else
                        r2[c] = 1.0 - (1.0 - r2[c]) * ((N - 1.0) / (N - P - 1.0));
                }

            return r2;
        }
    }
}