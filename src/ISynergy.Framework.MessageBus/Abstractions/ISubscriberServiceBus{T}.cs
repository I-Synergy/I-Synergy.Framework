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
        /// Subscribes to messagebus asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        Task SubscribeToMessageBusAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Unsubscribe from messagebus and closes connection.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UnSubscribeFromMessageBusAsync(CancellationToken cancellationToken = default);
    }
}
