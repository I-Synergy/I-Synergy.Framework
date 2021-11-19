namespace ISynergy.Framework.Mathematics.Optimization.Constrained.Constraints
{
    /// <summary>
    /// Defines an interface for an optimization constraint.
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        ///   Gets the type of the constraint.
        /// </summary>
        /// 
        ConstraintType ShouldBe { get; }

        /// <summary>
        ///   Gets the value in the right hand
        ///   side of the constraint equation.
        /// </summary>
        /// 
        double Value { get; }

        /// <summary>
        ///   Gets the violation tolerance for the constraint.
        /// </summary>
        /// 
        double Tolerance { get; }

        /// <summary>
        ///   Gets the number of variables in the constraint.
        /// </summary>
        /// 
        int NumberOfVariables { get; }

        /// <summary>
        /// Calculates the left hand side of the constraint
        /// equation given a vector x.
        /// </summary>
        /// <param name="x">The vector.</param>
        /// <returns>
        /// The left hand side of the constraint equation as evaluated at x.
        /// </returns>
        double Function(double[] x);

        /// <summary>
        /// Calculates the gradient of the constraint
        /// equation given a vector x
        /// </summary>
        /// <param name="x">The vector.</param>
        /// <returns>The gradient of the constraint as evaluated at x.</returns>
        double[] Gradient(double[] x);
    }
}
