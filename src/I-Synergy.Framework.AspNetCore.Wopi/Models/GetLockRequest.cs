using System.Net.Http;
using System.Net;

namespace ISynergy.Framework.Wopi.Models
{
    public class GetLockRequest : WopiRequest
    {
        internal GetLockRequest(HttpRequestMessage httpRequestMessage, string fileId) : base(httpRequestMessage, fileId)
        { }

        public GetLockResponse ResponseFileLocked(string existingLock)
        {
            return new GetLockResponse()
            {
                StatusCode = HttpStatusCode.OK,
                Lock = existingLock
            };
        }

        public GetLockResponse ResponseFileNotLocked()
        {
            return new GetLockResponse()
            {
                StatusCode = HttpStatusCode.OK,
                Lock = string.Empty
            };
        }

        public GetLockResponse ResponseLockConflict(string lockFailureReason = null)
        {
            return new GetLockResponse()
            {
                StatusCode = HttpStatusCode.Conflict,
                LockFailureReason = lockFailureReason
            };
        }

        public WopiResponse ResponseNotImplemented()
        {
            return new GetLockResponse()
            {
                StatusCode = HttpStatusCode.NotImplemented
            };
        }
    }
}