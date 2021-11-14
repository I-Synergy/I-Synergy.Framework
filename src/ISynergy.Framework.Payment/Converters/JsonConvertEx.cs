namespace ISynergy.Framework.Payment.Converters
{
    /// <summary>
    /// Class JsonConvertExtensions.
    /// </summary>
    public static class JsonConvertEx
    {
        /// <summary>
        /// Serializes the object camel case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string SerializeObjectCamelCase(object value)
        {
            return JsonConvert.SerializeObject(value,
                new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-dd",
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Serializes the object snake case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string SerializeObjectSnakeCase(object value)
        {
            return JsonConvert.SerializeObject(value,
                new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-dd",
                    ContractResolver = new SnakeCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }
    }
}
