using System.Text.Json.Serialization;

namespace ISynergy.Framework.MessageBus.Azure.Tests.Fixtures;

/// <summary>
/// Source-generated serializer context for the AOT constructor tests.
/// </summary>
[JsonSerializable(typeof(TestMessage))]
internal partial class TestJsonContext : JsonSerializerContext
{
}
