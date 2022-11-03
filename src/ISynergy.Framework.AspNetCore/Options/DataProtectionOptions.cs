using ISynergy.Framework.Storage.Abstractions.Options;

namespace ISynergy.Framework.AspNetCore.Options
{
    /// <summary>
    /// Class Azure Blob DataProtectionOptions.
    /// Implements the <see cref="IStorageOptions" />
    /// </summary>
    /// <seealso cref="IStorageOptions" />
    public class DataProtectionOptions : IStorageOptions
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }
    }
}
