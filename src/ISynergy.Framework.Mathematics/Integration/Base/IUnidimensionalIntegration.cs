namespace ISynergy.Framework.Mathematics.Integration.Base
{
    /// <summary>
    ///     Common interface for multidimensional integration methods.
    /// </summary>
    public interface IUnivariateIntegration : INumericalIntegration
    {
        /// <summary>
        ///     Gets or sets the unidimensional function
        ///     whose integral should be computed.
        /// </summary>
        Func<double, double> Function { get; set; }

        /// <summary>
        ///     Gets or sets the input range under
        ///     which the integral must be computed.
        /// </summary>
        NumericRange Range { get; set; }
    }
}