namespace ISynergy.Framework.Mathematics.Statistics
{
    /// <summary>
    ///   Common interface for convergence-based iterative learning algorithms.
    /// </summary>
    /// 
    public interface IConvergenceLearning
    {

        /// <summary>
        ///   Gets or sets the tolerance value used to determine 
        ///   whether the algorithm has converged. 
        /// </summary>
        /// 
        double Tolerance { get; set; }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the learning algorithm.
        /// </summary>
        /// 
        int MaxIterations { get; set; }

        /// <summary>
        ///   Gets the current iteration number.
        /// </summary>
        /// 
        int CurrentIteration { get; }

        /// <summary>
        ///   Gets or sets whether the algorithm has converged.
        /// </summary>
        /// 
        bool HasConverged { get; }
    }
}
