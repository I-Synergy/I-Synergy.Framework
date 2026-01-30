#if NET8_0_OR_GREATER
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Converters;

/// <summary>
/// A specialized converter for the <see cref="TimeOnly"/> type that
/// handles the specific requirements of the Datasync Toolkit.
/// </summary>
public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    private const string format = "HH:mm:ss.fff";

    /// <inheritdoc />
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => TimeOnly.Parse(reader.GetString() ?? string.Empty, CultureInfo.InvariantCulture);

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(format));
}
#endif