using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class CheckFileInfoRequest : WopiRequest
    {
        public CheckFileInfoRequest(HttpRequestMessage httpRequestMessage, string fileId) : base(httpRequestMessage, fileId)
        {
            SessionContext = GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.SESSION_CONTEXT);
        }
        public string SessionContext { get; private set; }

        public CheckFileInfoResponse ResponseOK(string baseFileName, string ownerId, long size, string userId, string version)
        {
            return new CheckFileInfoResponse()
            {
                StatusCode = HttpStatusCode.OK,
                BaseFileName = baseFileName,
                OwnerId = ownerId,
                Size = size,
                UserId = userId,
                Version = version
            };
        }
    }
}