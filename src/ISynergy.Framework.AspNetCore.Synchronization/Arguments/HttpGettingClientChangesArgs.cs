using ISynergy.Framework.AspNetCore.Synchronization.Cache;
using ISynergy.Framework.Synchronization.Client;
using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Enumerations;

namespace ISynergy.Framework.AspNetCore.Synchronization.Arguments
{
    public class HttpGettingClientChangesArgs : ProgressArgs
    {
        public HttpGettingClientChangesArgs(HttpMessageSendChangesRequest request, string host, SessionCache sessionCache)
            : base(request.SyncContext, null, null)
        {
            Request = request;
            Host = host;
            SessionCache = sessionCache;
        }

        public override string Source => Host;
        public override string Message
        {
            get
            {
                if (Request.BatchCount == 0 && Request.BatchIndex == 0)
                    return $"Getting All Changes. Rows:{Request.Changes.RowsCount()}";
                else
                    return $"Getting Batch Changes. ({Request.BatchIndex + 1}/{Request.BatchCount}). Rows:{Request.Changes.RowsCount()}";
            }
        }

        public HttpMessageSendChangesRequest Request { get; }
        public string Host { get; }
        public SessionCache SessionCache { get; }
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override int EventId => HttpServerSyncEventsId.HttpGettingChanges.Id;
    }
}
