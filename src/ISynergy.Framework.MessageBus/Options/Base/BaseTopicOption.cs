using ISynergy.Framework.MessageBus.Abstractions.Options;

namespace ISynergy.Framework.MessageBus.Options.Base;

/// <summary>
/// Class BaseTopicOption.
/// Implements the <see cref="ITopicOption" />
/// </summary>
/// <seealso cref="ITopicOption" />
public abstract class BaseTopicOption : ITopicOption
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    /// <value>The connection string.</value>
    public string ConnectionString { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the name of the topic.
    /// </summary>
    /// <value>The name of the topic.</value>
    public string TopicName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the name of the subscription.
    /// </summary>
    /// <value>The name of the subscription.</value>
    public string SubscriptionName { get; set; } = string.Empty;
}
