namespace ISynergy.Framework.Payment.Mollie
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The user agent header
        /// </summary>
        public const string UserAgentHeader = "User-Agent";
        /// <summary>
        /// The WWW URL content type
        /// </summary>
        public const string WWWUrlContentType = "application/x-www-form-urlencoded"; // http://tools.ietf.org/html/rfc4627
        /// <summary>
        /// The HTTP client default
        /// </summary>
        public const string HttpClientDefault = "Default";
        /// <summary>
        /// The HTTP client proxy
        /// </summary>
        public const string HttpClientProxy = "Proxy";
        /// <summary>
        /// The API endpoint
        /// </summary>
        public const string ApiEndpoint = "https://api.mollie.com/v2/";
    }
}
