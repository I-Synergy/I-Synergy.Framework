using ISynergy.Framework.MessageBus.Options.Base;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;

/// <summary>
/// Queue MessageBus publisher options for RabbitMQ.
/// </summary>
internal class PublisherOptions : BaseQueueOption
{
    /// <summary>
    /// Gets or sets the exchange name. Empty string uses the RabbitMQ default exchange.
    /// </summary>
    public string ExchangeName { get; set; } = string.Empty;
}
