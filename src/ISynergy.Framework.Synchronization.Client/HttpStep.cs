using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Synchronization.Client
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
