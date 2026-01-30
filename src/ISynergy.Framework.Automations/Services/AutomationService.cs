using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Automations.Options;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Automations.Services;

/// <summary>
/// The AutomationService class is responsible for orchestrating automation execution.
/// This service delegates to specialized services for condition validation, queue building, and action execution.
/// </summary>
public class AutomationService : IAutomationService
{
    private readonly ILogger _logger;
    private readonly AutomationOptions _options;
    private readonly IAutomationManager _manager;
    private readonly IAutomationConditionValidator _conditionValidator;
    private readonly IActionQueueBuilder _queueBuilder;
    private readonly List<Automation> _automations;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutomationService"/> class.
    /// </summary>
    /// <param name="manager">The automation manager for loading automations.</param>
    /// <param name="options">The automation options.</param>
    /// <param name="conditionValidator">The condition validator service.</param>
    /// <param name="queueBuilder">The action queue builder service.</param>
    /// <param name="logger">The logger.</param>
    public AutomationService(
        IAutomationManager manager,
        IOptions<AutomationOptions> options,
        IAutomationConditionValidator conditionValidator,
        IActionQueueBuilder queueBuilder,
        ILogger<AutomationService> logger)
    {
        _automations = new List<Automation>();
        _manager = manager;
        _options = options.Value;
        _conditionValidator = conditionValidator;
        _queueBuilder = queueBuilder;
        _logger = logger;
    }

    /// <summary>
    /// Gets all automations.
    /// </summary>
    /// <returns></returns>
    public async Task RefreshAutomationsAsync() =>
        _automations.AddNewRange(await _manager.GetItemsAsync());

    /// <summary>
    /// Validates the conditions of a given automation.
    /// </summary>
    /// <param name="automation">The automation to validate conditions for.</param>
    /// <param name="value">The value to validate against.</param>
    /// <returns>True if all conditions are met; otherwise, false.</returns>
    public Task<bool> ValidateConditionsAsync(Automation automation, object value) =>
        _conditionValidator.ValidateConditionsAsync(automation, value);

    /// <summary>
    /// Executes an automation by validating conditions and executing actions.
    /// </summary>
    /// <param name="automation">The automation to execute.</param>
    /// <param name="value">The value to pass to the automation.</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <returns>The result of the automation execution.</returns>
    public async Task<ActionResult> ExecuteAsync(Automation automation, object value, CancellationTokenSource cancellationTokenSource)
    {
        if (automation.IsActive && await ValidateConditionsAsync(automation, value))
        {
            cancellationTokenSource.CancelAfter(automation.ExecutionTimeout);

            using (var queue = await _queueBuilder.BuildQueueAsync(automation, value, cancellationTokenSource))
            {
                while (queue.Count > 0)
                {
                    var nextTask = queue.Take(cancellationTokenSource.Token);

                    if (cancellationTokenSource.IsCancellationRequested)
                        break;

                    await nextTask
                        .Invoke()
                        .ContinueWith(x => _logger.LogInformation("Task completed!"), cancellationTokenSource.Token);
                }

                return new ActionResult(true, value);
            }
        }

        return new ActionResult(false, value);
    }
}
