using ISynergy.Framework.Core.Constants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Converters
{
    public class IsoDateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            DateTime.ParseExact(reader.GetString(), StringFormats.IsoDateTimeFormat, null);

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(StringFormats.IsoDateTimeFormat));
        }
    }
}
