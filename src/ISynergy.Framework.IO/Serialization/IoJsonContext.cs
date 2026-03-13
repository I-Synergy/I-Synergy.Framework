using ISynergy.Framework.IO.Models;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.IO.Serialization;

/// <summary>
/// Source-generated <see cref="JsonSerializerContext"/> for types used in
/// <c>ISynergy.Framework.IO</c>. Replaces the reflection-based
/// <see cref="System.Text.Json.JsonSerializer.Deserialize{TValue}(string, System.Text.Json.JsonSerializerOptions?)"/>
/// overload so that JSON deserialization in <c>BaseFileTypeAnalyzer.LoadFileTypes</c>
/// is fully AOT-safe with no IL2026 trimmer warnings.
/// </summary>
[JsonSerializable(typeof(IEnumerable<FileTypeInfo>))]
[JsonSerializable(typeof(List<FileTypeInfo>))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
internal partial class IoJsonContext : JsonSerializerContext
{
}
