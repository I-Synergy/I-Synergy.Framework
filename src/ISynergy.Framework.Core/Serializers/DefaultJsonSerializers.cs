using ISynergy.Framework.Core.Converters;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Serializers;

public static class DefaultJsonSerializers
{
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "JsonStringEnumConverter is used in reflection-based serialization contexts. Use source-generated serialization for AOT scenarios.")]
    public static JsonSerializerOptions Default => new JsonSerializerOptions(JsonSerializerDefaults.General)
    {
        Converters =
        {
            new JsonStringEnumConverter(),
            new DateTimeConverter(),
            new DateTimeOffsetConverter(),
            new TimeOnlyConverter()
        },
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.Preserve
    };

    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "JsonStringEnumConverter is used in reflection-based serialization contexts. Use source-generated serialization for AOT scenarios.")]
    public static JsonSerializerOptions Web => new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        AllowTrailingCommas = true,
        Converters =
        {
            new JsonStringEnumConverter(),
            new DateTimeConverter(),
            new DateTimeOffsetConverter(),
            new TimeOnlyConverter()
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        IncludeFields = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true,
        ReferenceHandler = null
    };


}
