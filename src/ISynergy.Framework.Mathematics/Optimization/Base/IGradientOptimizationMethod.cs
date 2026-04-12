
using ISynergy.Framework.Mathematics.Optimization.Unconstrained;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

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
