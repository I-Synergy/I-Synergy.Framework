using ISynergy.Framework.Payment.Mollie.Abstractions.Services;
using ISynergy.Framework.Payment.Mollie.Converters;
using ISynergy.Framework.Payment.Mollie.Factories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Services
{
    /// <summary>
    /// Class JsonConverterService.
    /// Implements the <see cref="IJsonConverterService" />
    /// </summary>
    /// <seealso cref="IJsonConverterService" />
    public class JsonConverterService : IJsonConverterService
    {
        /// <summary>
        /// The default json deserializer settings
        /// </summary>
        private readonly JsonSerializerSettings _defaultJsonDeserializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConverterService" /> class.
        /// </summary>
        public JsonConverterService()
        {
            _defaultJsonDeserializerSettings = CreateDefaultJsonDeserializerSettings();
        }

        /// <summary>
        /// Serializes the specified object to serialize.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <returns>System.String.</returns>
        public string Serialize(object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize,
                new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-dd",
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Deserializes the specified json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns>T.</returns>
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _defaultJsonDeserializerSettings);
        }

        /// <summary>
        /// Creates the default Json serial settings for the JSON.NET parsing.
        /// </summary>
        /// <returns>JsonSerializerSettings.</returns>
        private JsonSerializerSettings CreateDefaultJsonDeserializerSettings()
        {
            return new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd",
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> {
                    // Add a special converter for payment responses, because we need to create specific classes based on the payment method
                    new PaymentResponseConverter(new PaymentResponseFactory())
                }
            };
        }
    }
}
