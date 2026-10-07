using ISynergy.Framework.Automations.Actions;

namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Automation service interface.
/// </summary>
public interface IAutomationService
{
    /// <summary>
    /// Gets all automations.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns></returns>
    Task RefreshAutomationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the conditions of a given automation.
    /// </summary>
    /// <param name="automation"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<bool> ValidateConditionsAsync(Automation automation, object value);

    /// <summary>
    /// Starts executing the automation.
    /// </summary>
    /// <param name="automation"></param>
    /// <param name="value"></param>
    /// <param name="cancellationTokenSource"></param>
    /// <returns></returns>
    Task<ActionResult> ExecuteAsync(Automation automation, object value, CancellationTokenSource cancellationTokenSource);
}
