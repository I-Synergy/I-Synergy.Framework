using System.Text.Json.Serialization.Metadata;
using ISynergy.Framework.MessageBus.Azure.Options.Queue;
using ISynergy.Framework.MessageBus.Azure.Services.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.MessageBus.Azure.Tests.Fixtures;

/// <summary>
/// Concrete subscriber using the AOT-safe source-generated deserialization constructor.
/// </summary>
internal class TestSubscriberServiceBusAot : SubscriberServiceBus<TestMessage, AzureSubscriberOptions>
{
    public TestSubscriberServiceBusAot(
        IOptions<AzureSubscriberOptions> options,
        ILogger<SubscriberServiceBus<TestMessage, AzureSubscriberOptions>> logger,
        JsonTypeInfo<TestMessage> jsonTypeInfo)
        : base(options, logger, jsonTypeInfo)
    {
    }

    public override Task<bool> ProcessDataAsync(TestMessage queueMessage) => Task.FromResult(true);

    public override bool ValidateMessage(TestMessage message) => message is not null;
}
