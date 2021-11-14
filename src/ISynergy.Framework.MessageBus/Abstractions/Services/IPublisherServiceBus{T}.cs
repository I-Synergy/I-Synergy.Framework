namespace ISynergy.Framework.MessageBus.Abstractions.Services
{
    /// <summary>
    /// Interface IPublisherServiceBus
    /// </summary>
    /// <typeparam name="TQueueMessage">The type of the t entity.</typeparam>
    public interface IPublisherServiceBus<TQueueMessage>
        where TQueueMessage : class, IBaseMessage
    {
        /// <summary>
        /// Sends the message asynchronous.
        /// </summary>
        /// <param name="queueMessage">The queue message.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>Task.</returns>
        Task SendMessageAsync(TQueueMessage queueMessage, Guid sessionId);
    }
}
