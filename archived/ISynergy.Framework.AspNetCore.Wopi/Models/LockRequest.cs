using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class LockRequest : WopiRequest
    {
        internal LockRequest(HttpRequestMessage httpRequestMessage, string fileId) : base(httpRequestMessage, fileId)
        {
            Lock = GetHttpRequestHeader(httpRequestMessage, WopiResponseHeaders.LOCK);
        }

        public string Lock { get; private set; }

        public LockResponse ResponseOK(string itemVersion = null)
        {
            return new LockResponse()
            {
                StatusCode = HttpStatusCode.OK,
                ItemVersion = itemVersion
            };
        }

        public LockResponse ResponseBadRequest()
        {
            return new LockResponse()
            {
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        public LockResponse ResponseLockConflict(string existingLock, string lockFailureReason = null)
        {
            return new LockResponse()
            {
                StatusCode = HttpStatusCode.Conflict,
                Lock = existingLock,
                LockFailureReason = lockFailureReason
            };
        }

        public WopiResponse ResponseNotImplemented()
        {
            return new LockResponse()
            {
                StatusCode = HttpStatusCode.NotImplemented
            };
        }
    }
}