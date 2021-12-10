namespace ISynergy.Framework.Synchronization.Core.Enumerations
{

    /// <summary>
    /// Defines where occured the exception
    /// </summary>
    public enum SyncSide
    {
        /// <summary>
        /// Occurs when error comes from LocalOrchestrator
        /// </summary>
        ClientSide,

        /// <summary>
        /// Occurs when error comes from RemoteOrchestrator
        /// </summary>
        ServerSide
    }
}
