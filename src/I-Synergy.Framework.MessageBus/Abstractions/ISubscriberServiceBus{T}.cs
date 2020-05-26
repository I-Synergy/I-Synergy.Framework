using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.MessageBus.Abstractions
{
    /// <summary>
    /// Interface ISubscriberServiceBus
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    public interface ISubscriberServiceBus<TEntity>
    {
        /// <summary>
        /// Subscribes to messages asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        Task SubscribeToMessagesAsync(CancellationToken cancellationToken = default);
    }
}
