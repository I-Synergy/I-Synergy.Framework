using ISynergy.Framework.Mathematics.Optimization.Constrained;
using ISynergy.Framework.Mathematics.Optimization.Unconstrained;

namespace ISynergy.Framework.Mathematics.Optimization.Base
{
    /// <summary>
    ///     Common interface for function optimization methods.
    /// </summary>
    /// <seealso cref="BoundedBroydenFletcherGoldfarbShanno" />
    /// <seealso cref="BroydenFletcherGoldfarbShanno" />
    /// <seealso cref="ConjugateGradient" />
    /// <seealso cref="ResilientBackpropagation" />
    /// <seealso cref="GoldfarbIdnani" />
    public interface IOptimizationMethod : IOptimizationMethod<double[], double>
    {
        // For backward compatibility
    }

    /// <summary>
    ///     Common interface for function optimization methods.
    /// </summary>
    /// <seealso cref="BoundedBroydenFletcherGoldfarbShanno" />
    /// <seealso cref="BroydenFletcherGoldfarbShanno" />
    /// <seealso cref="ConjugateGradient" />
    /// <seealso cref="ResilientBackpropagation" />
    /// <seealso cref="GoldfarbIdnani" />
    public interface IOptimizationMethod<TCode> : IOptimizationMethod, IOptimizationMethod<double[], double, TCode>
        where TCode : struct
    {
        // For backward compatibility
    }

    /// <summary>
    ///     Common interface for function optimization methods.
    /// </summary>
    /// <seealso cref="BoundedBroydenFletcherGoldfarbShanno" />
    /// <seealso cref="BroydenFletcherGoldfarbShanno" />
    /// <seealso cref="ConjugateGradient" />
    /// <seealso cref="ResilientBackpropagation" />
    /// <seealso cref="GoldfarbIdnani" />
    public interface IOptimizationMethod<TInput, TOutput>
    {
        /// <summary>
        ///     Gets the number of variables (free parameters)
        ///     in the optimization problem.
        /// </summary>
        /// <value>The number of parameters.</value>
        int NumberOfVariables { get; set; }

        /// <summary>
        ///     Gets the current solution found, the values of
        ///     the parameters which optimizes the function.
        /// </summary>
        TInput Solution { get; set; }

        /// <summary>
        ///     Gets the output of the function at the current <see cref="Solution" />.
        /// </summary>
        TOutput Value { get; }

        /// <summary>
        ///     Finds the minimum value of a function. The solution vector
        ///     will be made available at the <see cref="Solution" /> property.
        /// </summary>
        /// <returns>
        ///     Returns <c>true</c> if the method converged to a <see cref="Solution" />.
        ///     In this case, the found value will also be available at the <see cref="Value" />
        ///     property.
        /// </returns>
        bool Minimize();

        /// <summary>
        ///     Finds the maximum value of a function. The solution vector
        ///     will be made available at the <see cref="Solution" /> property.
        /// </summary>
        /// <returns>
        ///     Returns <c>true</c> if the method converged to a <see cref="Solution" />.
        ///     In this case, the found value will also be available at the <see cref="Value" />
        ///     property.
        /// </returns>
        bool Maximize();
    }
    /// <summary>
    ///     Common interface for function optimization methods.
    /// </summary>
    /// <seealso cref="BoundedBroydenFletcherGoldfarbShanno" />
    /// <seealso cref="BroydenFletcherGoldfarbShanno" />
    /// <seealso cref="ConjugateGradient" />
    /// <seealso cref="ResilientBackpropagation" />
    /// <seealso cref="GoldfarbIdnani" />
    public interface IOptimizationMethod<TInput, TOutput, TCode> : IOptimizationMethod<TInput, TOutput>
        where TCode : struct
    {
        /// <summary>
        ///     Get the exit code returned in the last call to the
        ///     <see cref="IOptimizationMethod{TInput, TOutput}.Maximize()" /> or
        ///     <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()" /> methods.
        /// </summary>
        TCode Status { get; }
    }
}