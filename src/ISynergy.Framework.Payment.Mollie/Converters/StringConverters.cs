using ISynergy.Framework.Payment.Extensions;

namespace ISynergy.Framework.Payment.Mollie.Converters
{
    /// <summary>
    /// Class StringConverters.
    /// </summary>
    public static class StringConverters
    {
        /// <summary>
        /// Builds the list query string.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="otherParameters">The other parameters.</param>
        /// <returns>System.String.</returns>
        public static string BuildListQueryString(string from, int? limit, IDictionary<string, string> otherParameters = null)
        {
            var queryParameters = new Dictionary<string, string>();
            queryParameters.AddValueIfNotNullOrEmpty(nameof(from), from);
            queryParameters.AddValueIfNotNullOrEmpty(nameof(limit), Convert.ToString(limit));

            if (otherParameters != null)
            {
                foreach (var parameter in otherParameters)
                {
                    queryParameters.AddValueIfNotNullOrEmpty(parameter.Key, parameter.Value);
                }
            }

            return queryParameters.ToQueryString();
        }
    }
}
