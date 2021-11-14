namespace ISynergy.Framework.MessageBus.Abstractions.Messages
{
    /// <summary>
    /// Queue Message.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    public interface IQueueMessage<TEntity> : IBaseMessage
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        TEntity Data { get; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>The action.</value>
        QueueMessageActions Action { get; }

        /// <summary>
        /// Gets to.
        /// </summary>
        /// <value>To.</value>
        string To { get; }

        /// <summary>
        /// Gets the reply to.
        /// </summary>
        /// <value>The reply to.</value>
        string ReplyTo { get; }
    }
}
