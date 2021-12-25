using ISynergy.Framework.AspNetCore.Synchronization.Cache;
using ISynergy.Framework.Synchronization.Client.Enumerations;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using Microsoft.AspNetCore.Http;

namespace ISynergy.Framework.AspNetCore.Synchronization.Arguments
{
    /// <summary>
    /// When Getting response from remote orchestrator
    /// </summary>
    public class HttpGettingRequestArgs : ProgressArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="context"></param>
        /// <param name="sessionCache"></param>
        /// <param name="httpStep"></param>
        public HttpGettingRequestArgs(HttpContext httpContext, SyncContext context, SessionCache sessionCache, HttpStep httpStep)
            : base(context, null, null)
        {
            HttpContext = httpContext;
            SessionCache = sessionCache;
            HttpStep = httpStep;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override int EventId => HttpServerSyncEventsId.HttpGettingRequest.Id;
        public HttpContext HttpContext { get; }
        public SessionCache SessionCache { get; }
        public HttpStep HttpStep { get; }
    }
}
