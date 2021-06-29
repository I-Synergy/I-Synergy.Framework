namespace ISynergy.Framework.Mathematics.Optimization
{
    using System;

    /// <summary>
    /// Extension methods on the <see cref="IConstraint"/> interface.
    /// </summary>
    public static class ConstraintExtensions
    {

        /// <summary>
        /// Gets how much the constraint is being violated.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <param name="input">The function point.</param>
        /// <returns>
        /// How much the constraint is being violated at the given point. Positive
        /// value means the constraint is not being violated with the returned slack,
        /// while a negative value means the constraint is being violated by the returned
        /// amount.
        /// </returns>
        public static double GetViolation(this IConstraint constraint, double[] input)
        {
            double fx = constraint.Function(input);

            switch (constraint.ShouldBe)
            {
                case ConstraintType.EqualTo:
                    return Math.Abs(fx - constraint.Value);

                case ConstraintType.GreaterThanOrEqualTo:
                    return fx - constraint.Value;

                case ConstraintType.LesserThanOrEqualTo:
                    return constraint.Value - fx;
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets whether this constraint is being violated
        /// (within the current tolerance threshold).
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <param name="input">The function point.</param>
        /// <returns>
        /// True if the constraint is being violated, false otherwise.
        /// </returns>
        public static bool IsViolated(this IConstraint constraint, double[] input)
        {
            return constraint.GetViolation(input) + constraint.Tolerance < 0;
        }
    }
}
