namespace ISynergy.Framework.Payment.Mollie.Abstractions.Services
{
    /// <summary>
    /// Interface IJsonConverterService
    /// </summary>
    public interface IJsonConverterService
    {
        /// <summary>
        /// Deserializes the specified json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns>T.</returns>
        T Deserialize<T>(string json);
        /// <summary>
        /// Serializes the specified object to serialize.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <returns>System.String.</returns>
        string Serialize(object objectToSerialize);
    }
}
