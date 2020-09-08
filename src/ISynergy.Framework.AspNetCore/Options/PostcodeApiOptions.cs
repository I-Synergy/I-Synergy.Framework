namespace ISynergy.Framework.AspNetCore.Options
{
    /// <summary>
    /// Class PostcodeApiOptions.
    /// </summary>
    public class PostcodeApiOptions
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the secret.
        /// </summary>
        /// <value>The secret.</value>
        public string Secret { get; set; } = string.Empty;
    }
}
