namespace ISynergy.Framework.Mathematics.Integration.Base
{
    /// <summary>
    ///     Common interface for multidimensional integration methods.
    /// </summary>
    public interface IMultidimensionalIntegration : INumericalIntegration
    {
        /// <summary>
        ///     Gets the number of parameters expected by
        ///     the <see cref="Function" /> to be integrated.
        /// </summary>
        int NumberOfParameters { get; }

        /// <summary>
        ///     Gets or sets the multidimensional function
        ///     whose integral should be computed.
        /// </summary>
        Func<double[], double> Function { get; set; }

        /// <summary>
        ///     Gets or sets the range of each input variable
        ///     under which the integral must be computed.
        /// </summary>
        NumericRange[] Range { get; }
    }
}