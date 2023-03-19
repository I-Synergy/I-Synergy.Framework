﻿using ISynergy.Framework.Mathematics.Convergence.Base;

namespace ISynergy.Framework.Mathematics.Convergence
{
    /// <summary>
    ///     Absolute convergence criteria.
    /// </summary>
    /// <remarks>
    ///     This class can be used to track progress and convergence
    ///     of methods which rely on the absolute change of a value.
    /// </remarks>
    /// <example>
    ///     <code>
    ///   // Create a new convergence criteria for a maximum of 10 iterations
    ///   var criteria = new AbsoluteConvergence(iterations: 10, tolerance: 0.1);
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
    ///   // maximum of 10 iterations with a final value
    ///   // of 1371.73:
    ///   
    ///   int iterations = criteria.CurrentIteration; // 10
    ///   double value = criteria.OldValue; // 1371.7333333
    /// </code>
    /// </example>
    public class AbsoluteConvergence : ISingleValueConvergence
    {
        private int maxIterations = 100;
        private double newValue;

        private double tolerance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AbsoluteConvergence" /> class.
        /// </summary>
        public AbsoluteConvergence()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AbsoluteConvergence" /> class.
        /// </summary>
        /// <param name="iterations">
        ///     The maximum number of iterations which should be
        ///     performed by the iterative algorithm. Setting to zero indicates there
        ///     is no maximum number of iterations. Default is 0.
        /// </param>
        /// <param name="tolerance">
        ///     The maximum change in the watched value
        ///     after an iteration of the algorithm used to detect convergence.
        ///     Default is 0.
        /// </param>
        /// <param name="startValue">
        ///     The initial value for the <see cref="NewValue" /> and
        ///     <see cref="OldValue" /> properties.
        /// </param>
        public AbsoluteConvergence(int iterations = 100, double tolerance = 0, double startValue = 0)
        {
            MaxIterations = iterations;
            this.tolerance = tolerance;
            newValue = startValue;
        }

        /// <summary>
        ///     Gets the watched value before the iteration.
        /// </summary>
        public double OldValue { get; private set; }
        /// <summary>
        ///     Gets or sets the maximum change in the watched value
        ///     after an iteration of the algorithm used to detect
        ///     convergence. Default is 0.
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
        ///     Gets or sets the current iteration number.
        /// </summary>
        public int CurrentIteration { get; set; }

        /// <summary>
        ///     Gets whether the algorithm has converged.
        /// </summary>
        public bool HasConverged
        {
            get
            {
                if (maxIterations > 0 && CurrentIteration >= maxIterations)
                    return true;

                if (tolerance > 0)
                {
                    // Stopping criteria is likelihood convergence
                    var delta = Math.Abs(OldValue - NewValue);

                    if (delta <= tolerance)
                        return true;
                }

                // Check if we have reached an invalid or perfectly separable answer
                if (double.IsNaN(NewValue) || double.IsInfinity(OldValue) && double.IsInfinity(NewValue))
                    return true;

                return false;
            }
        }
        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public void Clear()
        {
            CurrentIteration = 0;
            NewValue = 0;
            OldValue = 0;
        }
    }
}