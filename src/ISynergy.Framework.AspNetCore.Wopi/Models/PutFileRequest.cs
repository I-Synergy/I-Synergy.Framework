using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class PutFileRequest : WopiRequest
    {
        internal PutFileRequest(HttpRequestMessage httpRequestMessage, string fileId): base(httpRequestMessage, fileId)
        {
            Content = httpRequestMessage.Content;
            Lock = GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.LOCK);
        }
        public string Lock { get; private set; }

        public HttpContent Content { get; private set; }

        public PutFileResponse ResponseOK(string itemVersion = null)
        {
            return new PutFileResponse()
            {
                StatusCode = HttpStatusCode.OK,
                ItemVersion = itemVersion
            };
        }

        public PutFileResponse ResponseBadRequest()
        {
            return new PutFileResponse()
            {
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        public PutFileResponse ResponseLockConflict(string existingLock, string lockFailureReason = null)
        {
            return new PutFileResponse()
            {
                StatusCode = HttpStatusCode.Conflict,
                Lock = existingLock,
                LockFailureReason = lockFailureReason
            };
        }

        public PutFileResponse ResponseFileTooLarge()
        {
            return new PutFileResponse()
            {
                StatusCode = HttpStatusCode.RequestEntityTooLarge
            };
        }

        public WopiResponse ResponseNotImplemented()
        {
            return new PutFileResponse()
            {
                StatusCode = HttpStatusCode.NotImplemented
            };
        }
    }
}
