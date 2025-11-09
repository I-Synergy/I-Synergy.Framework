using ISynergy.Framework.Automations.Abstractions;

namespace ISynergy.Framework.Automations.Services.Operators;

/// <summary>
/// Strategy for OR operator.
/// </summary>
public class OrOperatorStrategy : IOperatorStrategy
{
    /// <summary>
    /// Applies the OR operator to combine two boolean values.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>The result of applying the OR operator (left || right).</returns>
    public bool Apply(bool left, bool right) => left || right;
}

