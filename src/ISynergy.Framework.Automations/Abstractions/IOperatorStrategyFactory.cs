using ISynergy.Framework.Automations.Enumerations;

namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Factory interface for resolving operator strategies.
/// </summary>
public interface IOperatorStrategyFactory
{
    /// <summary>
    /// Gets an operator strategy for the specified operator type.
    /// </summary>
    /// <param name="operatorType">The operator type.</param>
    /// <returns>An operator strategy for the specified type.</returns>
    IOperatorStrategy GetStrategy(OperatorTypes operatorType);
}

