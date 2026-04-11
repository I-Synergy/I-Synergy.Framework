using ISynergy.Framework.Core.Ranges;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.Integration.Base;

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