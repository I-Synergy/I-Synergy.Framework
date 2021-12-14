using ISynergy.Framework.AspNetCore.Synchronization.Cache;
using ISynergy.Framework.Synchronization.Client;
using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Enumerations;

namespace ISynergy.Framework.AspNetCore.Synchronization.Arguments
{
    public class HttpSendingServerChangesArgs : ProgressArgs
    {
        public HttpSendingServerChangesArgs(HttpMessageSendChangesResponse response, string host, SessionCache sessionCache, bool isSnapshot)
            : base(response.SyncContext, null, null)
        {
            Response = response;
            Host = host;
            SessionCache = sessionCache;
            IsSnapshot = isSnapshot;
        }

        public HttpMessageSendChangesResponse Response { get; }
        public string Host { get; }
        public SessionCache SessionCache { get; }
        public bool IsSnapshot { get; }
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override string Source => Host;
        public override string Message
        {
            get
            {
                var rowsCount = Response.Changes is null ? 0 : Response.Changes.RowsCount();
                var changesString = IsSnapshot ? "Snapshot" : "";

                if (Response.BatchCount == 0 && Response.BatchIndex == 0)
                    return $"Sending All {changesString} Changes. Rows:{rowsCount}";
                else
                    return $"Sending Batch {changesString} Changes. ({Response.BatchIndex + 1}/{Response.BatchCount}). Rows:{rowsCount}";
            }
        }

        public override int EventId => HttpServerSyncEventsId.HttpSendingChanges.Id;
    }
}
