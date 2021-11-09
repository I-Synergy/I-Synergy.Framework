namespace ISynergy.Services.Options.Base
{
    /// <summary>
    /// Base storage options class
    /// </summary>
    public abstract  class BaseStorageOptions : IStorageOptions
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }
    }
}
