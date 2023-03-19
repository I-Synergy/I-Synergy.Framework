using ISynergy.Framework.Automations.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Automations.BackgroundServices
{
    /// <summary>
    /// Background service where automations are run and monitored.
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
}
