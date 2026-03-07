using ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;
using ISynergy.Framework.MessageBus.RabbitMQ.Services.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Tests.Fixtures;

/// <summary>
/// Concrete publisher that exposes the protected PublisherServiceBus constructor for testing.
/// </summary>
internal class TestPublisherServiceBus : PublisherServiceBus<TestMessage, PublisherOptions>
{
    public TestPublisherServiceBus(
        IOptions<PublisherOptions> options,
        ILogger<PublisherServiceBus<TestMessage, PublisherOptions>> logger)
        : base(options, logger)
    {
    }
}
