using ISynergy.Framework.Core.Collections;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Converters
{
    public class TreeNodeJsonConverter<T> : JsonConverter<TreeNode<T>>
        where T : class
    {
        public override bool CanConvert(Type type)
        {
            return typeof(TreeNode<T>).IsAssignableFrom(type);
        }

        public override TreeNode<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonDocument.TryParseValue(ref reader, out var doc))
            {
                if (doc.RootElement.TryGetProperty("type", out var type))
                {
                    var typeValue = type.GetString();
                    var rootElement = doc.RootElement.GetRawText();
                    return JsonSerializer.Deserialize<TreeNode<T>>(rootElement, options);
                }

                throw new JsonException("Failed to extract type property, it might be missing?");
            }

            throw new JsonException("Failed to parse JsonDocument");
            {
            }
        }

        public override void Write(Utf8JsonWriter writer, TreeNode<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
