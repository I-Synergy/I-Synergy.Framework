using ISynergy.Framework.Automations.Abstractions;

namespace ISynergy.Framework.Automations.Services.Operators;

/// <summary>
/// Strategy for AND operator.
/// </summary>
public class AndOperatorStrategy : IOperatorStrategy
{
    /// <summary>
    /// Applies the AND operator to combine two boolean values.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>The result of applying the AND operator (left &amp;&amp; right).</returns>
    public bool Apply(bool left, bool right) => left && right;
}

