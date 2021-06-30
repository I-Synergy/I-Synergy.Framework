﻿using ISynergy.Framework.Mathematics.MachineLearning;
using ISynergy.Framework.Mathematics.Statistics;
using System;

namespace ISynergy.Framework.Mathematics.Optimization
{
    /// <summary>
    ///     Base class for least-squares optimizers implementing the <see cref="ILeastSquaresMethod" /> interface.
    /// </summary>
    /// <seealso cref="ParallelLearningBase" />
    /// <seealso cref="IConvergenceLearning" />
    public abstract class BaseLeastSquaresMethod : ParallelLearningBase, IConvergenceLearning
    {
        // Total of weights in the model
        private int numberOfParameters;

        private double[] solution;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseLeastSquaresMethod" /> class.
        /// </summary>
        public BaseLeastSquaresMethod()
        {
            Convergence = new RelativeConvergence(0, 1e-5);
        }


        /// <summary>
        ///     Gets or sets a parameterized model function mapping input vectors
        ///     into output values, whose optimum parameters must be found.
        /// </summary>
        /// <value>
        ///     The function to be optimized.
        /// </value>
        public LeastSquaresFunction Function { get; set; }


        /// <summary>
        ///     Gets or sets a function that computes the gradient vector in respect
        ///     to the function parameters, given a set of input and output values.
        /// </summary>
        /// <value>
        ///     The gradient function.
        /// </value>
        public LeastSquaresGradientFunction Gradient { get; set; }

        /// <summary>
        ///     Gets the solution found, the values of the parameters which
        ///     optimizes the function, in a least squares sense.
        /// </summary>
        public double[] Solution
        {
            get => solution;
            set
            {
                if (value.Length != numberOfParameters)
                    throw new ArgumentException("Parameter vectors must have the same length", "value");
                solution = value;
            }
        }

        /// <summary>
        ///     Gets the number of variables (free parameters) in the optimization problem.
        /// </summary>
        /// <value>
        ///     The number of parameters.
        /// </value>
        public int NumberOfParameters
        {
            get => numberOfParameters;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                init(value);
            }
        }

        /// <summary>
        ///     Gets the value at the solution found. This should be
        ///     the minimum value found for the objective function.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        ///     Gets or sets the convergence verification method.
        /// </summary>
        protected RelativeConvergence Convergence { get; set; }

        /// <summary>
        ///     Gets or sets the maximum number of iterations
        ///     performed by the iterative algorithm. Default
        ///     is 100.
        /// </summary>
        public int MaxIterations
        {
            get => Convergence.MaxIterations;
            set => Convergence.MaxIterations = value;
        }

        /// <summary>
        ///     Gets or sets the maximum relative change in the watched value
        ///     after an iteration of the algorithm used to detect convergence.
        ///     Default is zero.
        /// </summary>
        public double Tolerance
        {
            get => Convergence.Tolerance;
            set => Convergence.Tolerance = value;
        }

        /// <summary>
        ///     Gets the current iteration number.
        /// </summary>
        public int CurrentIteration
        {
            get => Convergence.CurrentIteration;
            set => Convergence.CurrentIteration = value;
        }

        /// <summary>
        ///     Gets whether the algorithm has converged.
        /// </summary>
        public bool HasConverged => Convergence.HasConverged;

        private void init(int numberOfParameters)
        {
            this.numberOfParameters = numberOfParameters;

            if (solution == null || solution.Length != numberOfParameters)
            {
                solution = new double[numberOfParameters];
                Initialize();
            }
        }

        /// <summary>
        ///     This method should be implemented by child classes to initialize
        ///     their fields once the <see cref="NumberOfParameters" /> is known.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        ///     Compute model error for a given data set.
        /// </summary>
        /// <param name="input">The input points.</param>
        /// <param name="output">The output points.</param>
        /// <returns>The sum of squared errors for the data.</returns>
        public double ComputeError(double[][] input, double[] output)
        {
            double sumOfSquaredErrors = 0;

            for (var i = 0; i < input.Length; i++)
            {
                var actual = Function(Solution, input[i]);
                var expected = output[i];

                var e = expected - actual;
                sumOfSquaredErrors += e * e;
            }

            return sumOfSquaredErrors / 2.0;
        }
    }
}