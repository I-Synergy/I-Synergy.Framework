using ISynergy.Framework.Mathematics.Convergence;
using ISynergy.Framework.Mathematics.Optimization.Base;
using System;
using System.ComponentModel;

namespace ISynergy.Framework.Mathematics.Optimization.Unconstrained
{
    /// <summary>
    ///     Gradient Descent (GD) for unconstrained optimization.
    /// </summary>
    /// <seealso cref="ConjugateGradient" />
    /// <seealso cref="BoundedBroydenFletcherGoldfarbShanno" />
    /// <seealso cref="BroydenFletcherGoldfarbShanno" />
    /// <seealso cref="TrustRegionNewtonMethod" />
    public class GradientDescent : BaseGradientOptimizationMethod, IGradientOptimizationMethod
    {
        private readonly RelativeConvergence convergence = new();

        private double eta = 1e-3;
        private readonly int numberOfUpdatesBeforeConvergenceCheck = 1;

        /// <summary>
        ///     Creates a new instance of the GD optimization algorithm.
        /// </summary>
        public GradientDescent()
        {
            Iterations = 0;
            Tolerance = 1e-5;
        }

        /// <summary>
        ///     Gets or sets the learning rate. Default is 1e-3.
        /// </summary>
        public double LearningRate
        {
            get => eta;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", "Learning rate should be higher than 0.");
                eta = value;
            }
        }

        /// <summary>
        ///     Gets or sets the maximum change in the average log-likelihood
        ///     after an iteration of the algorithm used to detect convergence.
        ///     Default is 1e-5.
        /// </summary>
        public double Tolerance
        {
            get => convergence.Tolerance;
            set => convergence.Tolerance = value;
        }

        /// <summary>
        ///     Gets or sets the maximum number of iterations
        ///     performed by the learning algorithm. Default is 0.
        /// </summary>
        public int Iterations
        {
            get => convergence.MaxIterations;
            set => convergence.MaxIterations = value;
        }

        /// <summary>
        ///     Occurs when the current learning progress has changed.
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        /// <summary>
        ///     Implements the actual optimization algorithm. This
        ///     method should try to minimize the objective function.
        /// </summary>
        protected override bool Optimize()
        {
            convergence.Clear();

            var updates = 0;

            do
            {
                if (Token.IsCancellationRequested)
                    break;

                var gradient = Gradient(Solution);
                for (var i = 0; i < Solution.Length; i++)
                    Solution[i] -= eta * gradient[i];

                updates++;

                if (updates >= numberOfUpdatesBeforeConvergenceCheck)
                {
                    convergence.NewValue = Function(Solution);
                    updates = 0;
                }
            } while (!convergence.HasConverged);

            return true;
        }

        /// <summary>
        ///     Raises the <see cref="E:ProgressChanged" /> event.
        /// </summary>
        /// <param name="args">The ProgressChangedEventArgs instance containing the event data.</param>
        protected void OnProgressChanged(ProgressChangedEventArgs args)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, args);
        }
    }
}