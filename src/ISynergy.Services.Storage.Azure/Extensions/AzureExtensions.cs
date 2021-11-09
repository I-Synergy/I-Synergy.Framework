namespace ISynergy.Data.Extensions
{
    /// <summary>
    /// Class AzureExtensions.
    /// </summary>
    public static class AzureExtensions
    {
        /// <summary>
        /// The BLOB test URL
        /// </summary>
        private const string BlobTestUrl = "isynergydocstest.blob.core.windows.net";
        /// <summary>
        /// The CDN test URL
        /// </summary>
        private const string CdnTestUrl = "docs-test.i-synergy.net";

        /// <summary>
        /// The BLOB URL
        /// </summary>
        private const string BlobUrl = "isynergydocs.blob.core.windows.net";
        /// <summary>
        /// The CDN URL
        /// </summary>
        private const string CdnUrl = "docs.i-synergy.net";

        /// <summary>
        /// Converts to azurecdnurl.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>System.String.</returns>
        public static string ToAzureCdnUrl(this string url)
        {
            if (url.Contains(BlobTestUrl))
                return url.Replace(BlobTestUrl, CdnTestUrl);

            if (url.Contains(BlobUrl))
                return url.Replace(BlobUrl, CdnUrl);

            return url;
        }
    }
}
