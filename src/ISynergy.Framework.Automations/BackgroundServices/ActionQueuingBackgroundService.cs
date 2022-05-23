using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Automations.BackgroundServices
{
    /// <summary>
    /// Background service where scheduled or delayed actions are monitored and executed.
    /// </summary>
    public class ActionQueuingBackgroundService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly AutomationOptions _options;
        private readonly IActionService _service;

        private Timer _backgroundTimer;
        private Timer _executionTimer;

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
        private async void RefreshQueue(object state)
        {
            await _service.RefreshTasksAsync();

            var result = await _service.CalculateTimespanAsync();

            if (result.Expiration != TimeSpan.Zero)
                _executionTimer = new Timer(ExecuteTask, result.UpcomingTask, TimeSpan.Zero, result.Expiration);
            else
                _executionTimer?.Change(Timeout.Infinite, 0);
        }

        /// <summary>
        /// Executes task when task delay has elapsed.
        /// </summary>
        /// <param name="state"></param>
        private async void ExecuteTask(object state) 
        { 
            if(state is IAction action) 
                await _service.ExcecuteActionAsync(action);
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
}
