using ISynergy.Framework.Mathematics.Differentiation;
using System;

namespace ISynergy.Framework.Mathematics.Optimization.Base
{
    /// <summary>
    ///     Base class for gradient-based optimization methods.
    /// </summary>
    public abstract class BaseGradientOptimizationMethod : BaseOptimizationMethod
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseGradientOptimizationMethod" /> class.
        /// </summary>
        protected BaseGradientOptimizationMethod()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseGradientOptimizationMethod" /> class.
        /// </summary>
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        protected BaseGradientOptimizationMethod(int numberOfVariables)
            : base(numberOfVariables)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseGradientOptimizationMethod" /> class.
        /// </summary>
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        /// <param name="function">The objective function whose optimum values should be found.</param>
        /// <param name="gradient">The gradient of the objective <paramref name="function" />.</param>
        protected BaseGradientOptimizationMethod(int numberOfVariables,
            Func<double[], double> function, Func<double[], double[]> gradient)
            : base(numberOfVariables, function)
        {
            if (gradient == null)
                throw new ArgumentNullException("gradient");

            Gradient = gradient;
        }

        /// <summary>
        ///     Gets or sets a function returning the gradient
        ///     vector of the function to be optimized for a
        ///     given value of its free parameters.
        /// </summary>
        /// <value>The gradient function.</value>
        public Func<double[], double[]> Gradient { get; set; }

        /// <summary>
        ///     Finds the maximum value of a function. The solution vector
        ///     will be made available at the <see cref="IOptimizationMethod{TInput, TOutput}.Solution" /> property.
        /// </summary>
        /// <returns>
        ///     Returns <c>true</c> if the method converged to a <see cref="IOptimizationMethod{TInput, TOutput}.Solution" />.
        ///     In this case, the found value will also be available at the
        ///     <see cref="IOptimizationMethod{TInput, TOutput}.Value" />
        ///     property.
        /// </returns>
        public override bool Maximize()
        {
            if (Gradient == null)
                Gradient = FiniteDifferences.Gradient(Function, NumberOfVariables);

            NonlinearObjectiveFunction.CheckGradient(Gradient, Solution);

            var g = Gradient;

            Gradient = x => g(x).Multiply(-1);

            var success = base.Maximize();

            Gradient = g;

            return success;
        }

        /// <summary>
        ///     Finds the minimum value of a function. The solution vector
        ///     will be made available at the <see cref="IOptimizationMethod{TInput, TOutput}.Solution" /> property.
        /// </summary>
        /// <returns>
        ///     Returns <c>true</c> if the method converged to a <see cref="IOptimizationMethod{TInput, TOutput}.Solution" />.
        ///     In this case, the found value will also be available at the
        ///     <see cref="IOptimizationMethod{TInput, TOutput}.Value" />
        ///     property.
        /// </returns>
        public override bool Minimize()
        {
            if (Gradient == null)
                Gradient = FiniteDifferences.Gradient(Function, NumberOfVariables);

            NonlinearObjectiveFunction.CheckGradient(Gradient, Solution);

            return base.Minimize();
        }
    }
}