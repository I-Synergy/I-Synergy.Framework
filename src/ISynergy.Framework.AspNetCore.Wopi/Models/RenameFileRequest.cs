using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class RenameFileRequest : WopiRequest
    {
        internal RenameFileRequest(HttpRequestMessage httpRequestMessage, string fileId) : base(httpRequestMessage, fileId)
        {
            RequestedName = GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.REQUESTED_NAME);
            Lock = GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.LOCK);
        }
        public string RequestedName { get; private set; }

        public string Lock { get; private set; }

        public RenameFileResponse ResponseOK(string renamedFileBaseName, string itemVersion = null)
        {
            return new RenameFileResponse()
            {
                StatusCode = HttpStatusCode.OK,
                RenamedFileBaseName = renamedFileBaseName,
                ItemVersion = itemVersion
            };
        }
        public RenameFileResponse ResponseLockConflict(string existingLock, string lockFailureReason = null)
        {
            return new RenameFileResponse()
            {
                StatusCode = HttpStatusCode.Conflict,
                Lock = existingLock,
                LockFailureReason = lockFailureReason
            };
        }
        public RenameFileResponse ResponseBadRequest(string invalidFileNameError)
        {
            return new RenameFileResponse()
            {
                StatusCode = HttpStatusCode.BadRequest,
                InvalidFileNameError = invalidFileNameError
            };
        }

        public WopiResponse ResponseNotImplemented()
        {
            return new RenameFileResponse()
            {
                StatusCode = HttpStatusCode.NotImplemented
            };
        }
    }
}
