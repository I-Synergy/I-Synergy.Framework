using ISynergy.Framework.Core.Constants;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Converters;

public class DateTimeConverter : JsonConverter<DateTime>
{
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
    private static readonly DateTimeStyles DefaultStyles = DateTimeStyles.None;

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? dateString = reader.GetString();

        if (string.IsNullOrEmpty(dateString))
            throw new FormatException("Expected date string value.");

        if (DateTime.TryParseExact(dateString, StringFormats.IsoDateTimeFormat,
            InvariantCulture, DefaultStyles, out DateTime result))
            return result;

        throw new FormatException($"Unable to parse \"{dateString}\" as a DateTime using format \"{StringFormats.IsoDateTimeFormat}\".");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        if (writer is null)
            throw new ArgumentNullException(nameof(writer));

        writer.WriteStringValue(value.ToString(StringFormats.IsoDateTimeFormat));
    }
}
