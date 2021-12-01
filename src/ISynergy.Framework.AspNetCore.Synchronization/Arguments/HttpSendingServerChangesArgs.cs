using ISynergy.Framework.AspNetCore.Synchronization.Cache;
using ISynergy.Framework.Synchronization.Client;
using ISynergy.Framework.Synchronization.Core.Arguments;

namespace ISynergy.Framework.AspNetCore.Synchronization.Arguments
{
    public class HttpSendingServerChangesArgs : ProgressArgs
    {
        public HttpSendingServerChangesArgs(HttpMessageSendChangesResponse response, string host, SessionCache sessionCache, bool isSnapshot)
            : base(response.SyncContext, null, null)
        {
            this.Response = response;
            this.Host = host;
            this.SessionCache = sessionCache;
            this.IsSnapshot = isSnapshot;
        }

        public HttpMessageSendChangesResponse Response { get; }
        public string Host { get; }
        public SessionCache SessionCache { get; }
        public bool IsSnapshot { get; }

        public override string Source => this.Host;
        public override string Message
        {
            get
            {
                var rowsCount = this.Response.Changes == null ? 0 : this.Response.Changes.RowsCount();
                var changesString = IsSnapshot ? "Snapshot" : "";

                if (this.Response.BatchCount == 0 && this.Response.BatchIndex == 0)
                    return $"Sending All {changesString} Changes. Rows:{rowsCount}";
                else
                    return $"Sending Batch {changesString} Changes. ({this.Response.BatchIndex + 1}/{this.Response.BatchCount}). Rows:{rowsCount}";
            }
        }

        public override int EventId => HttpServerSyncEventsId.HttpSendingChanges.Id;
    }
}
