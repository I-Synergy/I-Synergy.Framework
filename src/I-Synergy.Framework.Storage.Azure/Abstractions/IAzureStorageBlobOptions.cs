namespace ISynergy.Framework.Storage.Azure.Abstractions
{
    /// <summary>
    /// Interface IAzureStorageBlobOptions
    /// </summary>
    public interface IAzureStorageBlobOptions
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; set; }
    }
}
