using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;

namespace ISynergy.Framework.MessageBus.Azure.Tests.Fixtures;

/// <summary>
/// Minimal IBaseMessage implementation for unit testing.
/// </summary>
public class TestMessage : IBaseMessage
{
    public string Tag { get; set; } = "TestMessage";
    public string ContentType { get; set; } = "application/json";
}
