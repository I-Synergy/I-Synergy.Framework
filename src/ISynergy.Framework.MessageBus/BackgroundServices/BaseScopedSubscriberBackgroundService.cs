using ISynergy.Framework.MessageBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.MessageBus.BackgroundServices;

/// <summary>
/// Class BaseScopedSubscriberBackgroundService.
/// Implements the <see cref="BackgroundService" />
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
/// <typeparam name="TService">The type of the t service.</typeparam>
/// <seealso cref="BackgroundService" />
public abstract class BaseScopedSubscriberBackgroundService<TEntity, TService> : BackgroundService
    where TService : ISubscriberServiceBus<TEntity>
{
    /// <summary>
    /// The logger
    /// </summary>
    protected readonly ILogger _logger;
    /// <summary>
    /// The service provider
    /// </summary>
    protected readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseScopedSubscriberBackgroundService{TEntity, TService}" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="loggerFactory">The logger.</param>
    protected BaseScopedSubscriberBackgroundService(
        IServiceProvider serviceProvider,
        ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = loggerFactory.CreateLogger<BaseScopedSubscriberBackgroundService<TEntity, TService>>();
    }

    /// <summary>
    /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
    /// the lifetime of the long running operation(s) being performed.
    /// </summary>
    /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the long running operations.</returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedProcessingService =
            scope.ServiceProvider
                .GetRequiredService<TService>();

        return scopedProcessingService.SubscribeToMessageBusAsync(stoppingToken);
    }
}
