using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class GetFileResponse : WopiResponse
    {
        internal GetFileResponse()
        {
        }

        public HttpContent Content { get; internal set; }

        public string ItemVersion { get; set; }

        public override HttpResponseMessage ToHttpResponse()
        {
            var httpResponseMessage = base.ToHttpResponse();
            if (StatusCode == HttpStatusCode.OK)
            {
                SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.ITEM_VERSION, ItemVersion);
                httpResponseMessage.Content = Content;
            }
            return httpResponseMessage;
        }
    }
}