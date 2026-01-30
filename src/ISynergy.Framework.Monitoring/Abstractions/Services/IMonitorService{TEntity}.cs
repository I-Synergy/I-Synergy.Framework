namespace ISynergy.Framework.Monitoring.Abstractions.Services;

/// <summary>
/// Monitor hub service.
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
public interface IMonitorService<TEntity> where TEntity : class
{
    /// <summary>
    /// Publish to hub context.
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="eventname">The eventname.</param>
    /// <param name="data">The data.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    Task PublishAsync(string channel, string eventname, TEntity data, CancellationToken cancellationToken = default);
}
