﻿namespace ISynergy.Framework.Mathematics.Convergence
{
    /// <summary>
    ///     Relative convergence criteria.
    /// </summary>
    /// <remarks>
    ///     This class can be used to track progress and convergence
    ///     of methods which rely on the relative change of a value.
    /// </remarks>
    /// <example>
    ///     <code>
    ///   // Create a new convergence criteria with unlimited iterations
    ///   var criteria = new RelativeConvergence(iterations: 0, tolerance: 0.1);
    ///   
    ///   int progress = 1;
    ///   
    ///   do
    ///   {
    ///       // Do some processing...
    ///   
    ///   
    ///       // Update current iteration information:
    ///       criteria.NewValue = 12345.6 / progress++;
    ///   
    ///   } while (!criteria.HasConverged);
    ///   
    ///   
    ///   // The method will converge after reaching the 
    ///   // maximum of 11 iterations with a final value
    ///   // of 1234.56:
    ///   
    ///   int iterations = criteria.CurrentIteration; // 11
    ///   double value = criteria.OldValue; // 1234.56
    /// </code>
    /// </example>
    public class RelativeConvergence : ISingleValueConvergence
    {
        private int checks;
        private int maxChecks = 1;
        private int maxIterations = 100;
        private double newValue;

        private double tolerance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelativeConvergence" /> class.
        /// </summary>
        public RelativeConvergence()
        {
            init();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelativeConvergence" /> class.
        /// </summary>
        /// <param name="iterations">
        ///     The maximum number of iterations which should be
        ///     performed by the iterative algorithm. Setting to zero indicates there
        ///     is no maximum number of iterations. Default is 100.
        /// </param>
        /// <param name="tolerance">
        ///     The maximum relative change in the watched value
        ///     after an iteration of the algorithm used to detect convergence.
        ///     Default is 0.
        /// </param>
        public RelativeConvergence(int iterations, double tolerance)
        {
            init(iterations, tolerance);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelativeConvergence" /> class.
        /// </summary>
        /// <param name="iterations">
        ///     The maximum number of iterations which should be
        ///     performed by the iterative algorithm. Setting to zero indicates there
        ///     is no maximum number of iterations. Default is 0.
        /// </param>
        /// <param name="tolerance">
        ///     The maximum relative change in the watched value
        ///     after an iteration of the algorithm used to detect convergence.
        ///     Default is 0.
        /// </param>
        /// <param name="checks">
        ///     The minimum number of convergence checks that the
        ///     iterative algorithm should pass before convergence can be declared
        ///     reached.
        /// </param>
        public RelativeConvergence(int iterations, double tolerance, int checks)
        {
            init(iterations, tolerance, checks);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelativeConvergence" /> class.
        /// </summary>
        /// <param name="iterations">
        ///     The maximum number of iterations which should be
        ///     performed by the iterative algorithm. Setting to zero indicates there
        ///     is no maximum number of iterations. Default is 0.
        /// </param>
        /// <param name="tolerance">
        ///     The maximum relative change in the watched value
        ///     after an iteration of the algorithm used to detect convergence.
        ///     Default is 0.
        /// </param>
        /// <param name="checks">
        ///     The minimum number of convergence checks that the
        ///     iterative algorithm should pass before convergence can be declared
        ///     reached.
        /// </param>
        /// <param name="startValue">
        ///     The initial value for the <see cref="NewValue" /> and
        ///     <see cref="OldValue" /> properties.
        /// </param>
        public RelativeConvergence(int iterations = 100, double tolerance = 0, int checks = 1, double startValue = 0)
        {
            init(iterations, tolerance, checks, startValue);
        }

        /// <summary>
        ///     Gets or sets the watched value before the iteration.
        /// </summary>
        public double OldValue { get; private set; }

        /// <summary>
        ///     Gets the absolute difference between the <see cref="NewValue" /> and <see cref="OldValue" />
        ///     as as <c>Math.Abs(OldValue - NewValue)</c>.
        /// </summary>
        public double Delta => Math.Abs(OldValue - NewValue);

        /// <summary>
        ///     Gets the relative difference between the <see cref="NewValue" /> and <see cref="OldValue" />
        ///     as <c>Math.Abs(OldValue - NewValue) / Math.Abs(OldValue)</c>.
        /// </summary>
        public double RelativeDelta => Delta / Math.Abs(OldValue);

        /// <summary>
        ///     Gets the initial value for the <see cref="NewValue" />
        ///     and <see cref="OldValue" /> properties.
        /// </summary>
        public double StartValue { get; private set; }
        /// <summary>
        ///     Gets or sets the maximum relative change in the watched value
        ///     after an iteration of the algorithm used to detect convergence.
        ///     Default is zero.
        /// </summary>
        public double Tolerance
        {
            get => tolerance;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Tolerance should be positive.");

                tolerance = value;
            }
        }

        /// <summary>
        ///     Gets or sets the maximum number of iterations
        ///     performed by the iterative algorithm. Default
        ///     is 100.
        /// </summary>
        public int MaxIterations
        {
            get => maxIterations;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value",
                        "The maximum number of iterations should be positive.");

                maxIterations = value;
            }
        }

        /// <summary>
        ///     Gets or sets the watched value after the iteration.
        /// </summary>
        public double NewValue
        {
            get => newValue;
            set
            {
                OldValue = newValue;
                newValue = value;
                CurrentIteration++;
            }
        }

        /// <summary>
        ///     Gets the current iteration number.
        /// </summary>
        public int CurrentIteration { get; set; }

        /// <summary>
        ///     Gets whether the algorithm has converged.
        /// </summary>
        public bool HasConverged
        {
            get
            {
                var converged = checkConvergence();

                checks = converged ? checks + 1 : 0;

                return checks >= maxChecks;
            }
        }

        /// <summary>
        ///     Resets this instance, reverting all iteration statistics
        ///     statistics (number of iterations, last error) back to zero.
        /// </summary>
        public void Clear()
        {
            NewValue = StartValue;
            OldValue = StartValue;
            CurrentIteration = 0;
            checks = 0;
        }

        private void init(int iterations = 100, double tolerance = 0, int checks = 1, double startValue = 0)
        {
            MaxIterations = iterations;
            this.tolerance = tolerance;
            maxChecks = checks;
            StartValue = startValue;

            Clear();
        }

        private bool checkConvergence()
        {
            if (maxIterations > 0 && CurrentIteration >= maxIterations)
                return true;

            if (tolerance > 0)
                // Stopping criteria is likelihood convergence
                if (Delta <= tolerance * Math.Abs(OldValue))
                    return true;

            // Check if we have reached an invalid or perfectly separable answer
            if (double.IsNaN(NewValue) || double.IsInfinity(NewValue))
                return true;

            return false;
        }
        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (HasConverged)
                return string.Format("Converged: {0} < {1}", RelativeDelta, tolerance);
            return string.Format("Running: {0} > {1}", RelativeDelta, Tolerance);
        }
    }
}