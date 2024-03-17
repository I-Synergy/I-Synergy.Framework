namespace ISynergy.Framework.AspNetCore.WebDav.Storage.SQLite
{
    /// <summary>
    /// The options for the <see cref="SQLitePropertyStore"/>
    /// </summary>
    public class SQLitePropertyStoreOptions
    {
        /// <summary>
        /// Gets or sets the default estimated cost for querying the dead properties values
        /// </summary>
        public int EstimatedCost { get; set; } = 10;
    }
}
