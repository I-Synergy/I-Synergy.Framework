using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class DeleteFileRequest : WopiRequest
    {
        internal DeleteFileRequest(HttpRequestMessage httpRequestMessage, string fileId) : base(httpRequestMessage, fileId)
        {
        }
      
        public DeleteFileResponse ResponseOK()
        {
            return new DeleteFileResponse()
            {
                StatusCode = HttpStatusCode.OK
            };
        }
        public DeleteFileResponse ResponseLockConflict(string existingLock, string lockFailureReason = null)
        {
            return new DeleteFileResponse()
            {
                StatusCode = HttpStatusCode.Conflict,
                Lock = existingLock,
                LockFailureReason = lockFailureReason
            };
        }
        public WopiResponse ResponseNotImplemented()
        {
            return new DeleteFileResponse()
            {
                StatusCode = HttpStatusCode.NotImplemented
            };
        }
    }
}
