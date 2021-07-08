﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace ISynergy.Framework.Mathematics.Optimization
{
    /// <summary>
    ///     Nonlinear objective function.
    /// </summary>
    /// <example>
    ///     <para>
    ///         In this framework, it is possible to state a non-linear programming problem
    ///         using either symbolic processing or vector-valued functions. The following
    ///         example demonstrates the symbolic processing case:
    ///     </para>
    ///     <para>
    ///         And this is the same example as before, but using standard vectors instead.
    ///     </para>
    /// </example>
    /// <seealso cref="NelderMead" />
    /// <seealso cref="Cobyla" />
    /// <seealso cref="Subplex" />
    /// <seealso cref="AugmentedLagrangian" />
    /// <seealso cref="BroydenFletcherGoldfarbShanno" />
    public class NonlinearObjectiveFunction : IObjectiveFunction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NonlinearObjectiveFunction" /> class.
        /// </summary>
        protected NonlinearObjectiveFunction()
        {
            InnerVariables = new Dictionary<string, int>();
            InnerIndices = new Dictionary<int, string>();

            Variables = new ReadOnlyDictionary<string, int>(InnerVariables);
            Indices = new ReadOnlyDictionary<int, string>(InnerIndices);
        }

        /// <summary>
        ///     Creates a new objective function specified through a string.
        /// </summary>
        /// <param name="numberOfVariables">The number of parameters in the <paramref name="function" />.</param>
        /// <param name="function">
        ///     A lambda expression defining the objective
        ///     function.
        /// </param>
        public NonlinearObjectiveFunction(int numberOfVariables, Func<double[], double> function)
            : this()
        {
            NumberOfVariables = numberOfVariables;
            Function = function;

            for (var i = 0; i < numberOfVariables; i++)
            {
                var name = "x" + i;
                InnerVariables.Add(name, i);
                InnerIndices.Add(i, name);
            }
        }

        /// <summary>
        ///     Creates a new objective function specified through a string.
        /// </summary>
        /// <param name="numberOfVariables">The number of parameters in the <paramref name="function" />.</param>
        /// <param name="function">
        ///     A lambda expression defining the objective
        ///     function.
        /// </param>
        /// <param name="gradient">
        ///     A lambda expression defining the gradient
        ///     of the <paramref name="function">objective function</paramref>.
        /// </param>
        public NonlinearObjectiveFunction(int numberOfVariables,
            Func<double[], double> function, Func<double[], double[]> gradient)
            : this(numberOfVariables, function)
        {
            Gradient = gradient;
        }

#if !NET35
        /// <summary>
        ///     Creates a new objective function specified through a lambda expression.
        /// </summary>
        /// <param name="function">
        ///     A <see cref="Expression{TDelegate}" /> containing
        ///     the function in the form of a lambda expression.
        /// </param>
        /// <param name="gradient">
        ///     A <see cref="Expression{T}" /> containing
        ///     the gradient of the <paramref name="function">objective function</paramref>.
        /// </param>
        public NonlinearObjectiveFunction(Expression<Func<double>> function, Expression<Func<double[]>> gradient = null)
            : this()
        {
            var list = new SortedSet<string>();
            ExpressionParser.Parse(list, function.Body);

            var index = 0;
            foreach (var variable in list)
            {
                InnerVariables.Add(variable, index);
                InnerIndices.Add(index, variable);
                index++;
            }

            NumberOfVariables = index;

            // Generate lambda functions
            var func = ExpressionParser.Replace(function, InnerVariables);
            var grad = ExpressionParser.Replace(gradient, InnerVariables);

            Function = func.Compile();
            Gradient = grad.Compile();
        }
#endif

        /// <summary>
        ///     Gets the name of each input variable.
        /// </summary>
        protected Dictionary<string, int> InnerVariables { get; }

        /// <summary>
        ///     Gets the index of each input variable in the function.
        /// </summary>
        protected Dictionary<int, string> InnerIndices { get; }

        /// <summary>
        ///     Gets the gradient of the <see cref="Function">objective function</see>.
        /// </summary>
        public Func<double[], double[]> Gradient { get; protected set; }

        /// <summary>
        ///     Gets the name of each input variable.
        /// </summary>
        public IDictionary<string, int> Variables { get; }

        /// <summary>
        ///     Gets the index of each input variable in the function.
        /// </summary>
        public IDictionary<int, string> Indices { get; }

        /// <summary>
        ///     Gets the objective function.
        /// </summary>
        public Func<double[], double> Function { get; protected set; }
        /// <summary>
        ///     Gets the number of input variables for the function.
        /// </summary>
        public int NumberOfVariables { get; protected set; }
        internal static void CheckGradient(Func<double[], double[]> value, double[] probe)
        {
            var original = (double[])probe.Clone();
            var result = value(probe);

            if (result == probe)
                throw new InvalidOperationException(
                    "The gradient function should not return the parameter vector.");

            if (probe.Length != result.Length)
                throw new InvalidOperationException(
                    "The gradient vector should have the same length as the number of parameters.");

            for (var i = 0; i < probe.Length; i++)
                if (!probe[i].IsEqual(original[i]))
                    throw new InvalidOperationException("The gradient function shouldn't modify the parameter vector.");
        }
    }
}