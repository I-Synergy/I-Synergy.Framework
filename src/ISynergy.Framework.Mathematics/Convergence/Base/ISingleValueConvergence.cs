namespace ISynergy.Framework.Mathematics.Convergence.Base;

/// <summary>
///     Common interface for convergence detection algorithms that
///     depend solely on a single value (such as the iteration error).
/// </summary>
public interface ISingleValueConvergence : IConvergence<double>
{
}