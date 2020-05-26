namespace ISynergy.Framework.AspNetCore.Options
{
    /// <summary>
    /// Class AzureKeyVaultOptions.
    /// </summary>
    public class AzureKeyVaultOptions
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string ClientId { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        public string ClientSecret { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the key URI.
        /// </summary>
        /// <value>The key URI.</value>
        public string KeyUri { get; set; } = string.Empty;
    }
}
