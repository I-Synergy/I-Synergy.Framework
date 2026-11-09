
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Optimization.Unconstrained;

/// <summary>
///   Optimization progress event arguments.
/// </summary>
public class OptimizationProgressEventArgs : EventArgs
{
    /// <summary>
    ///   Gets the current iteration of the method.
    /// </summary>
    /// 
    public int Iteration { get; private set; }

    /// <summary>
    ///   Gets the number of function evaluations performed.
    /// </summary>
    /// 
    public int Evaluations { get; private set; }

    /// <summary>
    ///   Gets the current gradient of the function being optimized.
    /// </summary>
    /// 
    public double[] Gradient { get; private set; }

    /// <summary>
    ///   Gets the norm of the current <see cref="Gradient"/>.
    /// </summary>
    /// 
    public double GradientNorm { get; private set; }

    /// <summary>
    ///   Gets the current solution parameters for the problem.
    /// </summary>
    /// 
    public double[] Solution { get; private set; }

    /// <summary>
    ///   Gets the norm of the current <see cref="Solution"/>.
    /// </summary>
    /// 
    public double SolutionNorm { get; private set; }

    /// <summary>
    ///   Gets the value of the function to be optimized
    ///   at the current proposed <see cref="Solution"/>.
    /// </summary>
    /// 
    public double Value { get; private set; }

    /// <summary>
    ///   Gets the current step size.
    /// </summary>
    /// 
    public double Step { get; private set; }
    /// <summary>
    ///   Gets or sets a value indicating whether the
    ///   optimization process is about to terminate.
    /// </summary>
    /// 
    /// <value><c>true</c> if finished; otherwise, <c>false</c>.</value>
    /// 
    public bool Finished { get; private set; }

    /// <summary>
    ///   An user-defined value associated with this object.
    /// </summary>
    /// 
    public object Tag { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="OptimizationProgressEventArgs"/> class.
    /// </summary>
    /// 
    /// <param name="iteration">The current iteration of the optimization method.</param>
    /// <param name="evaluations">The number of function evaluations performed.</param>
    /// <param name="step">The current state (gradient, solution, value and step) of the optimization method.</param>
    /// <param name="finished"><c>True</c> if the method is about to terminate, <c>false</c> otherwise.</param>
    /// 
    public OptimizationProgressEventArgs(
        int iteration, int evaluations,
        OptimizationStep step, bool finished)
    {
        if (step.Gradient is not null)
            Gradient = (double[])step.Gradient.Clone();

        if (step.Solution is not null)
            Solution = (double[])step.Solution.Clone();

        Value = step.Value;
        GradientNorm = step.GradientNorm;
        SolutionNorm = step.SolutionNorm;

        Iteration = iteration;
        Evaluations = evaluations;

        Finished = finished;
        Step = step.Step;
    }
}

/// <summary>
///   Snapshot of the optimizer state reported through
///   <see cref="OptimizationProgressEventArgs"/>.
/// </summary>
public readonly struct OptimizationStep
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="OptimizationStep"/> struct.
    /// </summary>
    /// <param name="gradient">The current gradient of the function.</param>
    /// <param name="gnorm">The norm of the current gradient.</param>
    /// <param name="solution">The current solution parameters.</param>
    /// <param name="xnorm">The norm of the current parameter vector.</param>
    /// <param name="value">The value of the function evaluated at the current solution.</param>
    /// <param name="stp">The current step size.</param>
    public OptimizationStep(
        double[] gradient, double gnorm,
        double[] solution, double xnorm,
        double value, double stp)
    {
        Gradient = gradient;
        GradientNorm = gnorm;
        Solution = solution;
        SolutionNorm = xnorm;
        Value = value;
        Step = stp;
    }

    /// <summary>Gets the current gradient of the function.</summary>
    public double[] Gradient { get; }

    /// <summary>Gets the norm of the current gradient.</summary>
    public double GradientNorm { get; }

    /// <summary>Gets the current solution parameters.</summary>
    public double[] Solution { get; }

    /// <summary>Gets the norm of the current parameter vector.</summary>
    public double SolutionNorm { get; }

    /// <summary>Gets the value of the function at the current solution.</summary>
    public double Value { get; }

    /// <summary>Gets the current step size.</summary>
    public double Step { get; }
}
