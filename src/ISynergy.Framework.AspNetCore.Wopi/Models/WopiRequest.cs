using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace ISynergy.Framework.Wopi.Models
{
    public class WopiRequest
    {
        internal WopiRequest(HttpRequestMessage httpRequestMessage, string resourceId)
        {
            RequestUri = httpRequestMessage.RequestUri;
            ResourceId = resourceId;
            var queryStringParameters = HttpUtility.ParseQueryString(httpRequestMessage.RequestUri.AbsoluteUri);
            AccessToken = queryStringParameters[WopiQueryStrings.ACCESS_TOKEN];
            AppEndpoint = GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.APP_ENDPOINT);
            ClientVersion = GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.CLIENT_VERSION);
            CorrelationId = GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.CORRELATION_ID);
            DeviceId = GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.DEVICE_ID);
            MachineName = GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.MACHINE_NAME);
            Proof = GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.PROOF);
            ProofOld = GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.PROOF_OLD);
            Timestamp = GetHttpRequestHeader(httpRequestMessage, WopiRequestHeaders.TIME_STAMP);
        }
        public string ResourceId { get; private set; }
        public Uri RequestUri { get; private set; }
        public string AccessToken { get; private set; }
        public string AppEndpoint { get; private set; }
        public string ClientVersion { get; private set; }
        public string CorrelationId { get; private set; }
        public string DeviceId { get; private set; }
        public string MachineName { get; private set; }
        public string Proof { get; private set; }
        public string ProofOld { get; private set; }
        public string Timestamp { get; private set; }

        internal static string GetHttpRequestHeader(HttpRequestMessage httpRequestMessage, string header)
        {
            if (httpRequestMessage.Headers.TryGetValues(header, out IEnumerable<string> matches))
                return matches.FirstOrDefault();
            return null;
        }

       

        public WopiResponse ResponseServerError(string serverError)
        {
            return new WopiResponse()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ServerError = serverError
            };
        }

       
        public WopiResponse ResponseUnauthorized()
        {
            return new WopiResponse()
            {
                StatusCode = HttpStatusCode.Unauthorized
            };
        }

        public WopiResponse ResponseNotFound()
        {
            return new WopiResponse()
            {
                StatusCode = HttpStatusCode.NotFound
            };
        }

    }
}
