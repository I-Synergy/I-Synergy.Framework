using ISynergy.Framework.MessageBus.Tests.Fixtures;

namespace ISynergy.Framework.MessageBus.Tests.StepDefinitions;

/// <summary>
/// Shared context for MessageBus test scenarios.
/// Allows state sharing between different step definition classes.
/// </summary>
public class MessageBusTestContext
{
    public TestMessage? Message { get; set; }
    public List<TestMessage> PublishedMessages { get; set; } = new();
 public List<TestMessage> ReceivedMessages { get; set; } = new();
    public Dictionary<string, List<TestMessage>> TopicMessages { get; set; } = new();
    public Dictionary<string, int> SubscriberReceivedCounts { get; set; } = new();
    public Exception? CaughtException { get; set; }
    public bool IsSubscribed { get; set; }
    public string? CurrentTopic { get; set; }
}
