namespace ISynergy.Framework.MessageBus.Abstractions.Options
{
    /// <summary>
    /// Interface IQueueOption
    /// </summary>
    public interface IQueueOption
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; set; }
        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        /// <value>The name of the queue.</value>
        string QueueName { get; set; }
    }
}
