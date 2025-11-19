namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Interface for condition operator strategies.
/// </summary>
public interface IOperatorStrategy
{
    /// <summary>
    /// Applies the operator to combine two boolean values.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>The result of applying the operator.</returns>
    bool Apply(bool left, bool right);
}

