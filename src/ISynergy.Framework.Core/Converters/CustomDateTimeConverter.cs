using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Converters
{
    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string Format;
        
        public CustomDateTimeConverter(string format)
        {
            Format = format;
        }

        public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options) =>
            writer.WriteStringValue(date.ToString(Format));

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            DateTime.ParseExact(reader.GetString(), Format, null);
    }
}
