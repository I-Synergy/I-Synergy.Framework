using Sample.Models;
using System.Text.Json.Serialization;

namespace Sample.Serialization;

/// <summary>
/// Source-generated serializer context for settings persistence.
/// Required because PublishTrimmed disables reflection-based System.Text.Json.
/// </summary>
[JsonSerializable(typeof(LocalSettings))]
internal sealed partial class LocalSettingsJsonContext : JsonSerializerContext;
