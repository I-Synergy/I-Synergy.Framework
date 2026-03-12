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
#pragma warning disable IL2026, IL3050 // Reflection-based serializer used in tests; AOT not a test concern.
    public TestPublisherServiceBus(
        IOptions<PublisherOptions> options,
        ILogger<PublisherServiceBus<TestMessage, PublisherOptions>> logger)
        : base(options, logger)
    {
    }
#pragma warning restore IL2026, IL3050
}
