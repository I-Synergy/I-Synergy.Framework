namespace ISynergy.Services.Options
{
    /// <summary>
    /// Storage options interface.
    /// </summary>
    public interface IStorageOptions
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }
    }
}
