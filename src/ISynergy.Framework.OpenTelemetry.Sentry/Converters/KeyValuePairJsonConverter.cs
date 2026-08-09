using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.OpenTelemetry.Sentry.Converters;

/// <summary>
/// JSON converter factory for <see cref="KeyValuePair{TKey, TValue}"/> types.
/// System.Text.Json cannot serialize KeyValuePair natively because its constructor
/// parameter names are null. This converter handles the serialization as
/// {"Key": ..., "Value": ...}.
/// </summary>
/// <remarks>
/// This is registered globally because Sentry SDK uses its own internal
/// <see cref="JsonSerializerOptions"/> that we cannot customize directly.
/// Registering the converter on <see cref="JsonSerializerOptions.Default"/>
/// before Sentry initializes ensures the converter is available when Sentry
/// serializes device contexts (which may contain KeyValuePair values on iOS).
/// </remarks>
public sealed class KeyValuePairJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;

        return typeToConvert.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var keyType = typeToConvert.GetGenericArguments()[0];
        var valueType = typeToConvert.GetGenericArguments()[1];

        var converterType = typeof(KeyValuePairJsonConverter<,>).MakeGenericType(keyType, valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

/// <summary>
/// JSON converter for a specific <see cref="KeyValuePair{TKey, TValue}"/> type.
/// </summary>
public sealed class KeyValuePairJsonConverter<TKey, TValue> : JsonConverter<KeyValuePair<TKey, TValue>>
{
    /// <inheritdoc />
    public override KeyValuePair<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // This converter is only needed for serialization (Sentry writes, not reads).
        // Reading is unlikely to be called but we implement it for completeness.
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object");

        TKey key = default!;
        TValue value = default!;
        bool keySet = false;
        bool valueSet = false;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            var propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case "Key":
                    key = JsonSerializer.Deserialize<TKey>(ref reader, options)!;
                    keySet = true;
                    break;
                case "Value":
                    value = JsonSerializer.Deserialize<TValue>(ref reader, options)!;
                    valueSet = true;
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        if (!keySet || !valueSet)
            throw new JsonException("KeyValuePair JSON must contain both Key and Value properties");

        return new KeyValuePair<TKey, TValue>(key, value);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, KeyValuePair<TKey, TValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Key");
        JsonSerializer.Serialize(writer, value.Key, options);
        writer.WritePropertyName("Value");
        JsonSerializer.Serialize(writer, value.Value, options);
        writer.WriteEndObject();
    }
}
