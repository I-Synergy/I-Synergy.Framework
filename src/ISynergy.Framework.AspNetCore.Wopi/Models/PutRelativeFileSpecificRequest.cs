using System;
using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class PutRelativeFileSpecificRequest : WopiRequest
    {
        internal PutRelativeFileSpecificRequest(HttpRequestMessage httpRequestMessage, string fileId) : base(httpRequestMessage, fileId)
        {
            Content = httpRequestMessage.Content;

            RelativeTarget = WopiRequest.GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.RELATIVE_TARGET);
            var overwriteRelativeTargetStr =  WopiRequest.GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.OVERWRITE_RELATIVE_TARGET);
            if (RelativeTarget != null && overwriteRelativeTargetStr != null)
                OverwriteRelativeTarget = bool.Parse(overwriteRelativeTargetStr);
            var isFileConversionStr = WopiRequest.GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.FILE_CONVERSION);
            if (isFileConversionStr != null)
                IsFileConversion = bool.Parse(isFileConversionStr);
        }

        public HttpContent Content { get; private set; }
        public string RelativeTarget { get; private set; }
        public bool OverwriteRelativeTarget { get; private set; }
        public bool IsFileConversion { get; private set; }

        public PutRelativeFileResponse ResponseOK(string name, Uri url, Uri hostViewUrl = null, Uri hostEditUrl = null)
        {
            return new PutRelativeFileResponse()
            {
                StatusCode = HttpStatusCode.OK,
                Name = name,
                Url = url,
                HostViewUrl = hostViewUrl,
                HostEditUrl = hostEditUrl
            };
        }

        public PutRelativeFileResponse ResponseInvalidFileName(string validRelativeTarget = null)
        {
            return new PutRelativeFileResponse()
            {
                StatusCode = HttpStatusCode.BadRequest,
                ValidRelativeTarget = validRelativeTarget
            };
        }

        public PutRelativeFileResponse ResponseLockConflict(string existingLock, string lockFailureReason = null, string validRelativeTarget = null)
        {
            return new PutRelativeFileResponse()
            {
                StatusCode = HttpStatusCode.Conflict,
                Lock = existingLock,
                LockFailureReason = lockFailureReason,
                ValidRelativeTarget = validRelativeTarget
            };
        }

        public PutRelativeFileResponse ResponseBadRequest()
        {
            return new PutRelativeFileResponse()
            {
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        public WopiResponse ResponseNotImplemented()
        {
            return new PutRelativeFileResponse()
            {
                StatusCode = HttpStatusCode.NotImplemented
            };
        }
    }
}
