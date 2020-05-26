using System;
using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class PutRelativeFileSuggestedRequest : WopiRequest
    {
        internal PutRelativeFileSuggestedRequest(HttpRequestMessage httpRequestMessage, string fileId) : base(httpRequestMessage, fileId)
        {
            Content = httpRequestMessage.Content;

            SuggestedTarget = WopiRequest.GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.SUGGESTED_TARGET);
            var isFileConversionStr = WopiRequest.GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.FILE_CONVERSION);
            if (isFileConversionStr != null)
                IsFileConversion = bool.Parse(isFileConversionStr);
        }

        public HttpContent Content { get; private set; }
        public string SuggestedTarget { get; private set; }
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
        public WopiResponse ResponseNotImplemented()
        {
            return new PutRelativeFileResponse()
            {
                StatusCode = HttpStatusCode.NotImplemented
            };
        }

    }
}
