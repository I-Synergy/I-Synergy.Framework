using ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;
using ISynergy.Framework.MessageBus.RabbitMQ.Services.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Tests.Fixtures;

/// <summary>
/// Concrete subscriber that implements the abstract members for testing.
/// ProcessDataAsync returns true; ValidateMessage accepts any non-null message.
/// </summary>
internal class TestSubscriberServiceBus : SubscriberServiceBus<TestMessage, SubscriberOptions>
{
    public TestSubscriberServiceBus(
        IOptions<SubscriberOptions> options,
        ILogger<SubscriberServiceBus<TestMessage, SubscriberOptions>> logger)
        : base(options, logger)
    {
    }

    public override Task<bool> ProcessDataAsync(TestMessage queueMessage) =>
        Task.FromResult(true);

    public override bool ValidateMessage(TestMessage message) =>
        message is not null;
}
