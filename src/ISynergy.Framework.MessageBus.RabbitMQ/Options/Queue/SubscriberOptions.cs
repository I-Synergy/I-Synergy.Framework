using ISynergy.Framework.MessageBus.Options.Base;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;

/// <summary>
/// Queue MessageBus subscriber options for RabbitMQ.
/// </summary>
internal class SubscriberOptions : BaseQueueOption
{
    /// <summary>
    /// Gets or sets the exchange name. Empty string uses the RabbitMQ default exchange.
    /// </summary>
    public string ExchangeName { get; set; } = string.Empty;
}
