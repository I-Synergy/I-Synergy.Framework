using ISynergy.Framework.Automations.Actions;

namespace ISynergy.Framework.Automations.Abstractions
{
    /// <summary>
    /// Automation service interface.
    /// </summary>
    public interface IAutomationService
    {
        /// <summary>
        /// Gets all automations.
        /// </summary>
        /// <returns></returns>
        Task RefreshAutomationsAsync();

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
        /// <returns></returns>
        Task<ActionResult> ExecuteAsync(Automation automation, object value);
    }
}
