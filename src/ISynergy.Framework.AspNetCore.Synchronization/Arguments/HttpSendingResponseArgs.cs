using ISynergy.Framework.AspNetCore.Synchronization.Cache;
using ISynergy.Framework.Synchronization.Client;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Arguments;
using Microsoft.AspNetCore.Http;

namespace ISynergy.Framework.AspNetCore.Synchronization.Arguments
{
    /// <summary>
    /// When sending request 
    /// </summary>
    public class HttpSendingResponseArgs : ProgressArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="context"></param>
        /// <param name="sessionCache"></param>
        /// <param name="data"></param>
        /// <param name="httpStep"></param>
        public HttpSendingResponseArgs(HttpContext httpContext, SyncContext context, SessionCache sessionCache, byte[] data, HttpStep httpStep)
            : base(context, null, null)
        {
            HttpContext = httpContext;
            SessionCache = sessionCache;
            Data = data;
            HttpStep = httpStep;
        }

        public override int EventId => HttpServerSyncEventsId.HttpSendingResponse.Id;
        public HttpContext HttpContext { get; }
        public SessionCache SessionCache { get; }
        public byte[] Data { get; }
        public HttpStep HttpStep { get; }
    }
}
