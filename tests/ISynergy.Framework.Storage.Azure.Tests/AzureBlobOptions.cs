using ISynergy.Framework.Storage.Abstractions;

namespace ISynergy.Framework.Storage.Azure.Tests
{
    /// <summary>
    /// Class AzureBlobOptions.
    /// Implements the <see cref="ISynergy.Framework.Storage.Abstractions.IStorageOptions" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Storage.Abstractions.IStorageOptions" />
    public class AzureBlobOptions : IStorageOptions
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }
    }
}
