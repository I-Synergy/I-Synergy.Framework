using ISynergy.Framework.MessageBus.Abstractions.Options;

namespace ISynergy.Framework.MessageBus.Options.Base;

/// <summary>
/// Class BaseQueueOption.
/// Implements the <see cref="IQueueOption" />
/// </summary>
/// <seealso cref="IQueueOption" />
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
