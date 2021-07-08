namespace ISynergy.Framework.Mathematics.Optimization
{
    using System;

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
}
