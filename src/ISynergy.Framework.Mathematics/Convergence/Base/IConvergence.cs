namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    ///     Common interface for convergence detection algorithms.
    /// </summary>
    public interface IConvergence<T> : IConvergence
    {
        /// <summary>
        ///     Gets or sets the watched value after the iteration.
        /// </summary>
        T NewValue { get; set; }
    }

    /// <summary>
    ///     Common interface for convergence detection algorithms.
    /// </summary>
    public interface IConvergence
    {
        /// <summary>
        ///     Gets or sets the maximum relative change in the watched value
        ///     after an iteration of the algorithm used to detect convergence.
        /// </summary>
        double Tolerance { get; set; }

        /// <summary>
        ///     Gets or sets the maximum number of iterations
        ///     performed by the iterative algorithm.
        /// </summary>
        int MaxIterations { get; set; }

        /// <summary>
        ///     Gets the current iteration number.
        /// </summary>
        int CurrentIteration { get; }

        /// <summary>
        ///     Gets or sets whether the algorithm has converged.
        /// </summary>
        bool HasConverged { get; }

        /// <summary>
        ///     Resets this instance, reverting all iteration statistics
        ///     statistics (number of iterations, last error) back to zero.
        /// </summary>
        void Clear();
    }
}