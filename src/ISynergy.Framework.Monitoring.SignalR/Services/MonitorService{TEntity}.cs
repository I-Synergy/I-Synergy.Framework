using ISynergy.Framework.Monitoring.Abstractions.Services;
using ISynergy.Framework.Monitoring.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Monitoring.Services
{
    /// <summary>
    /// Monitor hub service.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    internal class MonitorService<TEntity> : IMonitorService<TEntity> where TEntity : class
    {
        /// <summary>
        /// The hub context
        /// </summary>
        private readonly IHubContext<MonitorHub> _hubContext;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="context">The context.</param>
        public MonitorService(IHubContext<MonitorHub> context)
        {
            _hubContext = context;
        }

        /// <summary>
        /// Publish to hub context.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="eventname">The eventname.</param>
        /// <param name="data">The data.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task PublishAsync(string channel, string eventname, TEntity data, CancellationToken cancellationToken = default) =>
            _hubContext.Clients.Group(channel).SendAsync(eventname, data, cancellationToken);
    }
}
