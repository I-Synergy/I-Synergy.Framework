using ISynergy.Framework.Storage.Abstractions.Options;

namespace ISynergy.Framework.Storage.Azure.Tests
{
    /// <summary>
    /// Class AzureBlobOptions.
    /// Implements the <see cref="Abstractions.Options.IStorageOptions" />
    /// </summary>
    /// <seealso cref="Abstractions.Options.IStorageOptions" />
    public class AzureBlobOptions : IStorageOptions
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }
    }
}
