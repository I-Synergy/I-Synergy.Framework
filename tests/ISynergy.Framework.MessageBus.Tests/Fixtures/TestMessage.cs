using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;

namespace ISynergy.Framework.MessageBus.Tests.Fixtures;

/// <summary>
/// Test message for BDD scenarios.
/// </summary>
public class TestMessage : IBaseMessage
{
    public string Tag { get; set; } = "TestMessage";
    public string ContentType { get; set; } = "application/json";
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Topic { get; set; } = string.Empty;
}
