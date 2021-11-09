using ISynergy.Services.Options;

namespace ISynergy.Services.Options
{
    /// <summary>
    /// Class Azure Options.
    /// </summary>
    public class AzureOptions
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        public string TenantId { get; set; }

        /// <summary>
        /// Class KeyVault.
        /// </summary>
        public class KeyVault
        {
            /// <summary>
            /// Gets or sets the key vault URI.
            /// </summary>
            /// <value>The key vault URI.</value>
            public Uri KeyVaultUri { get; set; }
            /// <summary>
            /// Gets or sets the client identifier.
            /// </summary>
            /// <value>The client identifier.</value>
            public string ClientId { get; set; }
            /// <summary>
            /// Gets or sets the client secret.
            /// </summary>
            /// <value>The client secret.</value>
            public string ClientSecret { get; set; }
        }

        /// <summary>
        /// Class Blob.
        /// </summary>
        public class Blob
        {
            /// <summary>
            /// Class DataProtection.
            /// Implements the <see cref="IStorageOptions" />
            /// </summary>
            /// <seealso cref="IStorageOptions" />
            public class DataProtection : IStorageOptions
            {
                /// <summary>
                /// Gets or sets the connection string.
                /// </summary>
                /// <value>The connection string.</value>
                public string ConnectionString { get; set; }
            }

            /// <summary>
            /// Class Documents.
            /// Implements the <see cref="IStorageOptions" />
            /// </summary>
            /// <seealso cref="IStorageOptions" />
            public class Documents : IStorageOptions
            {
                /// <summary>
                /// Gets or sets the connection string.
                /// </summary>
                /// <value>The connection string.</value>
                public string ConnectionString { get; set; }
            }
        }
    }
}
