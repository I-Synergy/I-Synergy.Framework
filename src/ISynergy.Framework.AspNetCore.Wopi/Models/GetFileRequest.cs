using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class GetFileRequest : WopiRequest
    {
        public GetFileRequest(HttpRequestMessage httpRequestMessage,string fileId) : base(httpRequestMessage, fileId)
        {
            IEnumerable<string> matchingHeaders;
            if (httpRequestMessage.Headers.TryGetValues(WopiRequestHeaders.MAX_EXPECTED_SIZE, out matchingHeaders))
                MaxExpectedSize = Int64.Parse(matchingHeaders.FirstOrDefault());
        }

        public long? MaxExpectedSize { get; private set; }

      
        public GetFileResponse ResponseOK(HttpContent content, string itemVersion = null)
        {
            return new GetFileResponse()
            {
                StatusCode = HttpStatusCode.OK,
                ItemVersion = itemVersion,
                Content = content
            };
        }

        public GetFileResponse ResponseFileTooLarge()
        {
            return new GetFileResponse()
            {
                StatusCode = HttpStatusCode.PreconditionFailed
            };
        }
    }
}