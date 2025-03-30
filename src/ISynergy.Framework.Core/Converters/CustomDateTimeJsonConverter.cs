using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Converters;

public class CustomDateTimeJsonConverter : JsonConverter<DateTime>
{
    private readonly string Format;

    public CustomDateTimeJsonConverter(string format)
    {
        Format = format;
    }

    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options) =>
        writer.WriteStringValue(date.ToString(Format));

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? dateString = reader.GetString();

        if (string.IsNullOrEmpty(dateString))
            throw new FormatException("Expected date string value.");

        if (DateTime.TryParseExact(dateString, Format,
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            return result;

        throw new FormatException($"Unable to parse \"{dateString}\" as a DateTime using format \"{Format}\".");
    }
}
