namespace ISynergy.Framework.Mathematics.Optimization.Base;

/// <summary>
///     Least Squares function delegate.
/// </summary>
/// <remarks>
///     This delegate represents a parameterized function that, given a set of
///     <paramref name="parameters" /> and an <paramref name="input" /> vector,
///     produces an associated output value.
/// </remarks>
/// <param name="parameters">The function parameters, also known as weights or coefficients.</param>
/// <param name="input">An input vector.</param>
/// <returns>
///     The output value produced given the <paramref name="input" /> vector
///     using the given <paramref name="parameters" />.
/// </returns>
public delegate double LeastSquaresFunction(double[] parameters, double[] input);

/// <summary>
///     Gradient function delegate.
/// </summary>
/// <remarks>
///     This delegate represents the gradient of a
///     <see cref="LeastSquaresFunction">
///         Least
///         Squares objective function
///     </see>
///     . This function should compute the gradient vector
///     in respect to the function <paramref name="parameters" />.
/// </remarks>
/// <param name="parameters">The function parameters, also known as weights or coefficients.</param>
/// <param name="input">An input vector.</param>
/// <param name="result">The resulting gradient vector (w.r.t to the parameters).</param>
public delegate void LeastSquaresGradientFunction(double[] parameters, double[] input, double[] result);
/// <summary>
///     Common interface for Least Squares algorithms, i.e. algorithms
///     that can be used to solve Least Squares optimization problems.
/// </summary>
/// <seealso cref="GaussNewton" />
/// <seealso cref="LevenbergMarquardt" />
public interface ILeastSquaresMethod
{
    /// <summary>
    ///     Gets or sets a cancellation token that can be used to
    ///     stop the learning algorithm while it is running.
    /// </summary>
    CancellationToken Token { get; set; }

    /// <summary>
    ///     Gets or sets a parameterized model function mapping input vectors
    ///     into output values, whose optimum parameters must be found.
    /// </summary>
    /// <value>The function to be optimized.</value>
    LeastSquaresFunction Function { get; set; }

    /// <summary>
    ///     Gets or sets a function that computes the gradient vector in respect
    ///     to the function parameters, given a set of input and output values.
    /// </summary>
    /// <value>The gradient function.</value>
    LeastSquaresGradientFunction Gradient { get; set; }

    /// <summary>
    ///     Gets the solution found, the values of the parameters which
    ///     optimizes the function, in a least squares sense.
    /// </summary>
    double[] Solution { get; set; }

    /// <summary>
    ///     Gets standard error for each parameter in the solution.
    /// </summary>
    double[] StandardErrors { get; }

    /// <summary>
    ///     Gets the value at the solution found. This should be
    ///     the minimum value found for the objective function.
    /// </summary>
    double Value { get; set; }

    /// <summary>
    ///     Attempts to find the best values for the parameter vector
    ///     minimizing the discrepancy between the generated outputs
    ///     and the expected outputs for a given set of input data.
    /// </summary>
    /// <param name="inputs">A set of input data.</param>
    /// <param name="outputs">
    ///     The values associated with each
    ///     vector in the <paramref name="inputs" /> data.
    /// </param>
    double Minimize(double[][] inputs, double[] outputs);
}