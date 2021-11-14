namespace ISynergy.Framework.Mathematics.Convergence
{
    /// <summary>
    ///     General convergence options.
    /// </summary>
    public class GeneralConvergence
    {
        /// <summary>
        ///     Creates a new <see cref="GeneralConvergence" /> object.
        /// </summary>
        /// <param name="numberOfVariables">The number of variables to be tracked.</param>
        public GeneralConvergence(int numberOfVariables)
        {
            NumberOfVariables = numberOfVariables;
            AbsoluteParameterTolerance = new double[NumberOfVariables];
        }

        /// <summary>
        ///     Gets or sets the number of variables in the problem.
        /// </summary>
        public int NumberOfVariables { get; set; }

        /// <summary>
        ///     Gets or sets the relative function tolerance that should
        ///     be used as convergence criteria. This tracks the relative
        ///     amount that the function output changes after two consecutive
        ///     iterations. Setting this value to zero disables those checks.
        ///     Default is 0.
        /// </summary>
        public double RelativeFunctionTolerance { get; set; }

        /// <summary>
        ///     Gets or sets the absolute function tolerance that should
        ///     be used as convergence criteria. This tracks the absolute
        ///     amount that the function output changes after two consecutive
        ///     iterations. Setting this value to zero disables those checks.
        ///     Default is 0.
        /// </summary>
        public double AbsoluteFunctionTolerance { get; set; }

        /// <summary>
        ///     Gets or sets the relative parameter tolerance that should
        ///     be used as convergence criteria. This tracks the relative
        ///     amount that the model parameters changes after two consecutive
        ///     iterations. Setting this value to zero disables those checks.
        ///     Default is 0.
        /// </summary>
        public double RelativeParameterTolerance { get; set; }

        /// <summary>
        ///     Gets or sets the absolute parameter tolerance that should
        ///     be used as convergence criteria. This tracks the absolute
        ///     amount that the model parameters changes after two consecutive
        ///     iterations. Setting this value to zero disables those checks.
        ///     Default is 0.
        /// </summary>
        public double[] AbsoluteParameterTolerance { get; set; }

        /// <summary>
        ///     Gets or sets the number of function evaluations
        ///     performed by the optimization algorithm.
        /// </summary>
        public int Evaluations { get; set; }

        /// <summary>
        ///     Gets or sets the maximum number of function evaluations to
        ///     be used as convergence criteria. This tracks how many times
        ///     the function to be optimized has been called, and stops the
        ///     algorithm when the number of times specified in this property
        ///     has been reached. Setting this value to zero disables this check.
        ///     Default is 0.
        /// </summary>
        public int MaximumEvaluations { get; set; }

        /// <summary>
        ///     Gets or sets the maximum amount of time that an optimization
        ///     algorithm is allowed to run. This property must be set together
        ///     with <see cref="StartTime" /> in order to function correctly.
        ///     Setting this value to <see cref="TimeSpan.Zero" /> disables this
        ///     check. Default is <see cref="TimeSpan.Zero" />.
        /// </summary>
        public TimeSpan MaximumTime { get; set; }

        /// <summary>
        ///     Gets or sets the time when the algorithm started running. When
        ///     time will be tracked with the <see cref="MaximumTime" /> property,
        ///     this property must also be set to a correct value.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     Gets or sets whether the algorithm should
        ///     be forced to terminate. Default is false.
        /// </summary>
        public bool Cancel { get; set; }
    }
}