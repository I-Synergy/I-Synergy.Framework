
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Integration.Base;

/// <summary>
///     Common interface for numeric integration methods.
/// </summary>
public interface INumericalIntegration : ICloneable
{
    /// <summary>
    ///     Gets the numerically computed result of the
    ///     definite integral for the specified function.
    /// </summary>
    double Area { get; }

    /// <summary>
    ///     Computes the area of the function under the selected
    ///     range. The computed value will be available at this
    ///     class's <see cref="Area" /> property.
    /// </summary>
    /// <returns>True if the integration method succeeds, false otherwise.</returns>
    bool Compute();
}

/// <summary>
///     Common interface for numeric integration methods.
/// </summary>
public interface INumericalIntegration<TCode> : INumericalIntegration
    where TCode : struct
{
    /// <summary>
    ///     Get the exit code returned in the last call to the
    ///     <see cref="INumericalIntegration.Compute()" /> method.
    /// </summary>
    TCode Status { get; }
}