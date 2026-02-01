using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Automations.BackgroundServices;

/// <summary>
/// This class implements a background service for scheduling and executing actions.
/// It takes in an IActionService, AutomationOptions, and an ILogger in its constructor.The IActionService provides methods for refreshing, calculating timespans, and executing actions.The AutomationOptions contains settings like the default queue refresh rate.The ILogger is used to log information.
/// In the StartAsync method, it logs a message about starting the service, calls the IActionService to refresh the tasks, and starts a Timer to periodically call the RefreshQueue method based on the default queue refresh rate from AutomationOptions.
/// The RefreshQueue method calls the IActionService to refresh the tasks again.It then calculates the timespan until the next upcoming task using the service.If there is an upcoming task, it starts a Timer to call the ExecuteTask method when the timespan expires.Otherwise, it stops any running timer.
/// The ExecuteTask method simply calls the IActionService to execute the passed in IAction task that is ready to run based on its elapsed delay.
/// So in summary, this background service periodically checks for any queued actions that are ready to execute based on their scheduled time, and runs them when ready.It uses the IActionService to handle the actual task management and execution. The background and execution timers drive the periodic refreshing and execution based on configured intervals.
/// </summary>
public class ActionQueuingBackgroundService : IHostedService, IDisposable
{
    private readonly ILogger _logger;
    private readonly AutomationOptions _options;
    private readonly IActionService _service;

    private Timer? _backgroundTimer;
    private Timer? _executionTimer;

    /// <summary>
    /// Default constructor for the background service.
    /// </summary>
    /// <param name="actionService"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public ActionQueuingBackgroundService(
        IActionService actionService,
        IOptions<AutomationOptions> options,
        ILogger<ActionQueuingBackgroundService> logger)
    {
        _service = actionService;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Starts the background service.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting scheduled action queuing service.");
        await _service.RefreshTasksAsync();
        _backgroundTimer = new Timer(RefreshQueue, null, TimeSpan.Zero, _options.DefaultQueueRefreshRate);
    }

    /// <summary>
    /// Stops the background service.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Scheduled action queuing service is stopping.");
        _backgroundTimer?.Change(Timeout.Infinite, 0);
        _executionTimer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }



    /// <summary>
    /// Refreshes the queue and sets the execution timer to the first available tasks.
    /// </summary>
    /// <param name="state"></param>
    private async void RefreshQueue(object? state)
    {
        try
        {
            await _service.RefreshTasksAsync();

            var result = await _service.CalculateTimespanAsync();

            if (result.HasValue && result.Value.Expiration != TimeSpan.Zero)
                _executionTimer = new Timer(ExecuteTask, result.Value.UpcomingTask, TimeSpan.Zero, result.Value.Expiration);
            else
                _executionTimer?.Change(Timeout.Infinite, 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing automation queue");
            // Re-throw to crash the application if unhandled exception handler is configured
            throw;
        }
    }

    /// <summary>
    /// Executes task when task delay has elapsed.
    /// </summary>
    /// <param name="state"></param>
    private async void ExecuteTask(object? state)
    {
        try
        {
            if (state is IAction action)
                await _service.ExcecuteActionAsync(action);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing automation action");
            // Re-throw to crash the application if unhandled exception handler is configured
            throw;
        }
    }

    /// <summary>
    /// Disposes timers.
    /// </summary>
    public void Dispose()
    {
        _executionTimer?.Dispose();
        _backgroundTimer?.Dispose();
    }
}
