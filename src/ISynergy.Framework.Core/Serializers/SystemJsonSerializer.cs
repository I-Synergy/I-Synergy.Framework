using Flurl.Http.Configuration;
using ISynergy.Framework.Core.Converters;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Serializers
{
    /// <summary>
    /// ISerializer implementation for use with Flurl.
    /// see https://github.com/tmenier/Flurl/issues/517#issuecomment-821541278
    /// </summary>
    public class SystemJsonSerializer : ISerializer
    {
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="options"></param>
        public SystemJsonSerializer(JsonSerializerOptions options = null)
        {
            if (options is null)
                options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                    ReferenceHandler = null
                };

            _options = options;
            _options.Converters.Add(new JsonStringEnumConverter());
            _options.Converters.Add(new IsoDateTimeJsonConverter());
            _options.Converters.Add(new IsoDateTimeOffsetJsonConverter());
        }

        /// <summary>
        /// Deserialize string to T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public T Deserialize<T>(string s)
        {
            if (string.IsNullOrEmpty(s))
                return default;
            return JsonSerializer.Deserialize<T>(s, _options);
        }


        /// <summary>
        /// Deserialize stream to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public T Deserialize<T>(Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return default;
            return JsonSerializer.Deserialize<T>(stream, _options);
        }

        /// <summary>
        /// Serialize object to json.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize(object obj) =>
            JsonSerializer.Serialize(obj, _options);
    }
}
