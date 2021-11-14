namespace ISynergy.Framework.Payment.Extensions
{
    /// <summary>
    /// Class DictionaryExtensions.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Converts to querystring.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public static string ToQueryString(this Dictionary<string, string> parameters)
        {
            if (!parameters.Any())
                return string.Empty;

            return "?" + string.Join("&", parameters.Select(x => $"{WebUtility.UrlEncode(x.Key)}={WebUtility.UrlEncode(x.Value)}"));
        }

        /// <summary>
        /// Adds the value if not null or empty.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void AddValueIfNotNullOrEmpty(this IDictionary<string, string> dictionary, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                dictionary.Add(key, value);
            }
        }
    }
}
