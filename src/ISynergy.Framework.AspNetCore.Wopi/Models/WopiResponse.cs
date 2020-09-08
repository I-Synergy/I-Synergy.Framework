using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{

    [JsonObject(MemberSerialization.OptIn)]
    public class WopiResponse
    {
        internal WopiResponse()
        {
            HostEndpoint = WopiConfiguration.HostEndpoint;
            MachineName = WopiConfiguration.MachineName;
            ServerVersion = WopiConfiguration.ServerVersion;
        }
        
        public HttpStatusCode StatusCode { get; internal set; }

        public virtual HttpResponseMessage ToHttpResponse()
        {
            var httpResponseMessage = new HttpResponseMessage(StatusCode);
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.HOST_ENDPOINT, HostEndpoint);
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.MACHINE_NAME, MachineName);
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.SERVER_VERSION, ServerVersion);
            return httpResponseMessage;
        }

        protected void SetHttpResponseHeader(HttpResponseMessage httpResponseMessage, string header, string value)
        {
            if (header != null && value != null)
                httpResponseMessage.Headers.Add(header, value);
        }

        public string HostEndpoint { get; internal set; }
        public string MachineName { get; internal set; }
        public string ServerVersion { get; internal set; }
        public string ServerError { get; internal set; }
    }
}