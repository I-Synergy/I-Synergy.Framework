using ISynergy.Framework.Automations.Actions;

namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Interface for validating automation conditions.
/// </summary>
public interface IAutomationConditionValidator
{
    /// <summary>
    /// Validates the conditions of a given automation.
    /// </summary>
    /// <param name="automation">The automation to validate conditions for.</param>
    /// <param name="value">The value to validate against.</param>
    /// <returns>True if all conditions are met; otherwise, false.</returns>
    Task<bool> ValidateConditionsAsync(Automation automation, object value);
}

