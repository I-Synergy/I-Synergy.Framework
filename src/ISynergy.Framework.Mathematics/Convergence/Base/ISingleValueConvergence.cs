
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Convergence.Base;

/// <summary>
///     Common interface for convergence detection algorithms that
///     depend solely on a single value (such as the iteration error).
/// </summary>
public interface ISingleValueConvergence : IConvergence<double>
{
}