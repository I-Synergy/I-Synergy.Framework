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
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
    private static readonly DateTimeStyles DefaultStyles = DateTimeStyles.None;
    private static readonly DateTimeStyles LenientStyles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;


    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();

        if (string.IsNullOrEmpty(dateString))
            throw new FormatException("Expected date string value.");

        // Try parsing with the specific format first
        if (DateTimeOffset.TryParseExact(dateString, StringFormats.IsoDateTimeFormat,
            InvariantCulture, DefaultStyles, out DateTimeOffset result))
            return result;

        // If that fails, try a more lenient parse with ISO 8601 formats
        if (DateTimeOffset.TryParse(dateString, InvariantCulture,
            LenientStyles, out result))
            return result;

        throw new FormatException($"Unable to parse \"{dateString}\" as a DateTimeOffset using format \"{StringFormats.IsoDateTimeFormat}\".");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        if (writer is null)
            throw new ArgumentNullException(nameof(writer));

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
