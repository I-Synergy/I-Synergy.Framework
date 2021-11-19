using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace ISynergy.Framework.Core.Converters
{
    /// <summary>
    /// Class IsoDateTimeOffsetConverter.
    /// Implements the <see cref="IsoDateTimeConverter" />
    /// </summary>
    /// <seealso cref="IsoDateTimeConverter" />
    public class IsoDateTimeOffsetConverter : IsoDateTimeConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?);

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                if (dateTimeOffset.Offset == TimeSpan.Zero)
                {
                    // If there is no offset, serialize as a DateTime
                    base.WriteJson(writer, dateTimeOffset.UtcDateTime, serializer);
                }
                else
                {
                    base.WriteJson(writer, value, serializer);
                }
            }
        }
    }
}
