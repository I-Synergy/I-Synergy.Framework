namespace ISynergy.Framework.MessageBus.Abstractions
{
    /// <summary>
    /// Interface ITopicOption
    /// </summary>
    public interface ITopicOption
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; set; }
        /// <summary>
        /// Gets or sets the name of the topic.
        /// </summary>
        /// <value>The name of the topic.</value>
        string TopicName { get; set; }
        /// <summary>
        /// Gets or sets the name of the subscription.
        /// </summary>
        /// <value>The name of the subscription.</value>
        string SubscriptionName { get; set; }
    }
}
