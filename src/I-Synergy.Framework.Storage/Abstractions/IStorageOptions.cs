namespace ISynergy.Framework.Storage.Abstractions
{
    /// <summary>
    /// Interface IAzureStorageBlobOptions
    /// </summary>
    public interface IStorageOptions
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; set; }
    }
}
