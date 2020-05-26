namespace ISynergy.Framework.AspNetCore.Options
{
    /// <summary>
    /// Class AzureBlobOptions.
    /// </summary>
    public class AzureBlobOptions
    {
        /// <summary>
        /// Gets or sets the name of the account.
        /// </summary>
        /// <value>The name of the account.</value>
        public string AccountName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the account key.
        /// </summary>
        /// <value>The account key.</value>
        public string AccountKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// Class AzureDocumentOptions.
    /// </summary>
    public class AzureDocumentOptions
    {
        /// <summary>
        /// Gets or sets the name of the account.
        /// </summary>
        /// <value>The name of the account.</value>
        public string AccountName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the account key.
        /// </summary>
        /// <value>The account key.</value>
        public string AccountKey { get; set; } = string.Empty;
    }
}
