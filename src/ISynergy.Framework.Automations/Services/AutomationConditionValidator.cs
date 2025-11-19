using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Framework.Automations.Services;

/// <summary>
/// Service responsible for validating automation conditions.
/// </summary>
public class AutomationConditionValidator : IAutomationConditionValidator
{
    private readonly IOperatorStrategyFactory _operatorStrategyFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutomationConditionValidator"/> class.
    /// </summary>
    /// <param name="operatorStrategyFactory">The factory for resolving operator strategies.</param>
    public AutomationConditionValidator(IOperatorStrategyFactory operatorStrategyFactory)
    {
        _operatorStrategyFactory = operatorStrategyFactory;
    }

    /// <summary>
    /// Validates the conditions of a given automation.
    /// </summary>
    /// <param name="automation">The automation to validate conditions for.</param>
    /// <param name="value">The value to validate against.</param>
    /// <returns>True if all conditions are met; otherwise, false.</returns>
    public Task<bool> ValidateConditionsAsync(Automation automation, object value)
    {
        var areAllConditionsValid = true;

        // Check if all conditions met.
        foreach (var condition in automation.Conditions.EnsureNotNull())
        {
            var operatorStrategy = _operatorStrategyFactory.GetStrategy(condition.Operator);
            var conditionResult = condition.ValidateCondition(value);
            areAllConditionsValid = operatorStrategy.Apply(areAllConditionsValid, conditionResult);
        }

        return Task.FromResult(areAllConditionsValid);
    }
}

