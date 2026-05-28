using System.Text.Json.Serialization;

namespace ISynergy.Framework.OpenTelemetry.Serialization;

/// <summary>
/// Source-generated <see cref="JsonSerializerContext"/> for telemetry serialization.
/// </summary>
/// <remarks>
/// Provides AOT (Ahead-of-Time) compilation compatible JSON serialization for profile
/// collection types used by <see cref="Processors.UserContextEnrichingLogProcessor"/>.
/// </remarks>
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(IEnumerable<string>))]
internal partial class TelemetryJsonContext : JsonSerializerContext
{
}
