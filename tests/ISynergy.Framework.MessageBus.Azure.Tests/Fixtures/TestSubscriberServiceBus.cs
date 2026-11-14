using ISynergy.Framework.MessageBus.Azure.Options.Queue;
using ISynergy.Framework.MessageBus.Azure.Services.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.MessageBus.Azure.Tests.Fixtures;

/// <summary>
/// Concrete subscriber that implements the abstract members for testing.
/// </summary>
internal class TestSubscriberServiceBus : SubscriberServiceBus<TestMessage, AzureSubscriberOptions>
{
    public TestSubscriberServiceBus(
        IOptions<AzureSubscriberOptions> options,
        ILogger<SubscriberServiceBus<TestMessage, AzureSubscriberOptions>> logger)
        : base(options, logger)
    {
    }

    public override Task<bool> ProcessDataAsync(TestMessage queueMessage) => Task.FromResult(true);

    public override bool ValidateMessage(TestMessage message) => message is not null;
}
