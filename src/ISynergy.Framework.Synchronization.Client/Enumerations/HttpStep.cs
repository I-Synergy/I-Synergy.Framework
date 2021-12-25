namespace ISynergy.Framework.Synchronization.Client.Enumerations
{
    /// <summary>
    /// Http steps involved during a sync beetween a proxy client and proxy server
    /// </summary>
    public enum HttpStep
    {
        None,
        EnsureSchema,
        EnsureScopes,
        SendChanges,
        SendChangesInProgress,
        GetChanges,
        GetEstimatedChangesCount,
        GetMoreChanges,
        GetChangesInProgress,
        GetSnapshot,
        GetSummary,
        SendEndDownloadChanges
    }
}
