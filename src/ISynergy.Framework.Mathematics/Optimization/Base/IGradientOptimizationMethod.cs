
using ISynergy.Framework.Mathematics.Optimization.Unconstrained;
namespace ISynergy.Framework.Mathematics.Optimization.Base;

/// <summary>
///   Common interface for function optimization methods which depend on
///   having both an objective function and a gradient function definition
///   available.
/// </summary>
/// 
/// <seealso cref="BroydenFletcherGoldfarbShanno"/>
/// <seealso cref="ConjugateGradient"/>
/// <seealso cref="ResilientBackpropagation"/>
/// 
public interface IGradientOptimizationMethod : IOptimizationMethod, IGradientOptimizationMethod<double[], double>
{
    // For backward compatibility

}

/// <summary>
///   Common interface for function optimization methods which depend on
///   having both an objective function and a gradient function definition
///   available.
/// </summary>
/// 
/// <seealso cref="BroydenFletcherGoldfarbShanno"/>
/// <seealso cref="ConjugateGradient"/>
/// <seealso cref="ResilientBackpropagation"/>
/// 
public interface IGradientOptimizationMethod<TInput, TOutput> : IFunctionOptimizationMethod<TInput, TOutput>
{
    /// <summary>
    ///   Gets or sets a function returning the gradient
    ///   vector of the function to be optimized for a
    ///   given value of its free parameters.
    /// </summary>
    /// 
    /// <value>The gradient function.</value>
    /// 
    Func<TInput, TInput> Gradient { get; set; }

}
