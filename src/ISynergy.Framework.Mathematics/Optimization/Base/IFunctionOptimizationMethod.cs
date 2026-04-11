
using ISynergy.Framework.Mathematics.Optimization.Constrained;
using ISynergy.Framework.Mathematics.Optimization.Unconstrained;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.Optimization.Base;

/// <summary>
///   Common interface for function optimization methods.
/// </summary>
/// 
/// <seealso cref="BoundedBroydenFletcherGoldfarbShanno"/>
/// <seealso cref="BroydenFletcherGoldfarbShanno"/>
/// <seealso cref="ConjugateGradient"/>
/// <seealso cref="ResilientBackpropagation"/>
/// <seealso cref="GoldfarbIdnani"/>
/// 
public interface IFunctionOptimizationMethod<TInput, TOutput> : IOptimizationMethod<TInput, TOutput>
{

    /// <summary>
    ///   Gets or sets the function to be optimized.
    /// </summary>
    /// 
    /// <value>The function to be optimized.</value>
    /// 
    Func<TInput, TOutput> Function { get; set; }

}
