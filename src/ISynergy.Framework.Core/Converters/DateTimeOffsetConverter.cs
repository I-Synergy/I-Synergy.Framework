using ISynergy.Framework.Core.Constants;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Converters;

/// <summary>
/// Class IsoDateTimeOffsetConverter.
/// Implements the <see cref="DateTimeConverter" />
/// </summary>
/// <seealso cref="DateTimeConverter" />
public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? dateString = reader.GetString();

        if (string.IsNullOrEmpty(dateString))
            throw new FormatException("Expected date string value.");

        if (DateTimeOffset.TryParseExact(dateString, StringFormats.IsoDateTimeFormat,
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result))
            return result;

        throw new FormatException($"Unable to parse \"{dateString}\" as a DateTimeOffset using format \"{StringFormats.IsoDateTimeFormat}\".");
    }

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
