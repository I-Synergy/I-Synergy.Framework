using ISynergy.Framework.MessageBus.Options.Queue;
using ISynergy.Framework.MessageBus.Services.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Tests.Fixtures;

/// <summary>
/// Concrete subscriber that implements the abstract members for testing.
/// ProcessDataAsync returns true; ValidateMessage accepts any non-null message.
/// </summary>
internal class TestSubscriberServiceBus : SubscriberServiceBus<TestMessage, SubscriberOptions>
{
#pragma warning disable IL2026, IL3050 // Reflection-based deserializer used in tests; AOT not a test concern.
    public TestSubscriberServiceBus(
        IOptions<SubscriberOptions> options,
        ILogger<SubscriberServiceBus<TestMessage, SubscriberOptions>> logger)
        : base(options, logger)
    {
    }
#pragma warning restore IL2026, IL3050

    public override Task<bool> ProcessDataAsync(TestMessage queueMessage) =>
        Task.FromResult(true);

    public override bool ValidateMessage(TestMessage message) =>
        message is not null;
}
