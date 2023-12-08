using ISynergy.Framework.Core.Constants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Converters;

/// <summary>
/// Class IsoDateTimeOffsetConverter.
/// Implements the <see cref="IsoDateTimeJsonConverter" />
/// </summary>
/// <seealso cref="IsoDateTimeJsonConverter" />
public class IsoDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        DateTimeOffset.ParseExact(reader.GetString(), StringFormats.IsoDateTimeFormat, null);

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        if (value.Offset == TimeSpan.Zero)
        {
            // If there is no offset, serialize as a DateTime
            writer.WriteStringValue(value.UtcDateTime.ToString(StringFormats.IsoDateTimeFormat));
        }
        else
        {
            writer.WriteStringValue(value.ToString(StringFormats.IsoDateTimeFormat));
        }
    }
}
