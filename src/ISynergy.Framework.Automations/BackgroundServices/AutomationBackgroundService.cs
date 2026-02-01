using ISynergy.Framework.Automations.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Automations.BackgroundServices;

/// <summary>
/// This is a background service where automations are run and monitored in an application.
/// It takes in an IAutomationService and ILogger in the constructor.The IAutomationService is used to run and refresh the automations.The ILogger is used to log information messages.
/// The StartAsync method starts up the background service. It logs a message that it is starting, and then calls the IAutomationService to refresh the list of automations.
/// The StopAsync method stops the background service.It just logs a message that it is stopping and returns.
/// The main logic is in StartAsync where it initializes the automation service to start running the automations. The StopAsync method allows gracefully shutting down the background service.
/// The purpose is to have a hosted background service that can run and monitor any automations in an application. It abstracts the initialization and running of the automations into a background service.This allows the rest of the application to not worry about setting up and running the automations.
/// </summary>
public class AutomationBackgroundService : IHostedService, IDisposable
{
    private readonly ILogger _logger;
    private readonly IAutomationService _service;


    /// <summary>
    /// Default constructor for all automations.
    /// </summary>
    public AutomationBackgroundService(
        IAutomationService automationService,
        ILogger<AutomationBackgroundService> logger)
    {
        _service = automationService;
        _logger = logger;
    }

    /// <summary>
    /// Starts the automation background service.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting automation service.");
        await _service.RefreshAutomationsAsync();
    }

    /// <summary>
    /// Stops the automation background service.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Automation service is stopping.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
