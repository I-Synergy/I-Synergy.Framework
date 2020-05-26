using ISynergy.Framework.MessageBus.Abstractions;

namespace ISynergy.Framework.MessageBus.Options
{
    /// <summary>
    /// Class BaseQueueOption.
    /// Implements the <see cref="ISynergy.Framework.MessageBus.Abstractions.IQueueOption" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.MessageBus.Abstractions.IQueueOption" />
    public abstract class BaseQueueOption : IQueueOption
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        /// <value>The name of the queue.</value>
        public string QueueName { get; set; } = string.Empty;
    }
}
