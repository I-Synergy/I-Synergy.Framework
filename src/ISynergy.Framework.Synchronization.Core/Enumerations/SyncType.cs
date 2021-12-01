namespace ISynergy.Framework.Synchronization.Core.Enumerations
{
    /// <summary>
    /// Synchronization mode. Could be Normal, Reinitialize, ReinitializeWithUpload
    /// </summary>
    public enum SyncType
    {
        /// <summary>
        /// Normal synchronization
        /// </summary>
        Normal,
        /// <summary>
        /// Reinitialize the whole sync database, applying all rows from the server to the client
        /// </summary>
        Reinitialize,
        /// <summary>
        /// Reinitialize the whole sync database, applying all rows from the server to the client, after trying a client upload
        /// </summary>
        ReinitializeWithUpload
    }
}
