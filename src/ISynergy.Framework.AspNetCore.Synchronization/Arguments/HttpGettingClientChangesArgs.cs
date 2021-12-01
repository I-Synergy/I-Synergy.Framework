using ISynergy.Framework.AspNetCore.Synchronization.Cache;
using ISynergy.Framework.Synchronization.Client;
using ISynergy.Framework.Synchronization.Core.Arguments;

namespace ISynergy.Framework.AspNetCore.Synchronization.Arguments
{
    public class HttpGettingClientChangesArgs : ProgressArgs
    {
        public HttpGettingClientChangesArgs(HttpMessageSendChangesRequest request, string host, SessionCache sessionCache)
            : base(request.SyncContext, null, null)
        {
            this.Request = request;
            this.Host = host;
            this.SessionCache = sessionCache;
        }

        public override string Source => this.Host;
        public override string Message
        {
            get
            {
                if (this.Request.BatchCount == 0 && this.Request.BatchIndex == 0)
                    return $"Getting All Changes. Rows:{this.Request.Changes.RowsCount()}";
                else
                    return $"Getting Batch Changes. ({this.Request.BatchIndex + 1}/{this.Request.BatchCount}). Rows:{this.Request.Changes.RowsCount()}";
            }
        }

        public HttpMessageSendChangesRequest Request { get; }
        public string Host { get; }
        public SessionCache SessionCache { get; }

        public override int EventId => HttpServerSyncEventsId.HttpGettingChanges.Id;
    }
}
