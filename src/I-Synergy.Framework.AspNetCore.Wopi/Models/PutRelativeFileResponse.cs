using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PutRelativeFileResponse : WopiResponse
    {
       internal PutRelativeFileResponse()
        { }

        [JsonProperty]
        public string Name { get; internal set; }
        [JsonProperty]
        public Uri Url { get; internal set; }
        [JsonProperty]
        public Uri HostViewUrl { get; set; }
        [JsonProperty]
        public Uri HostEditUrl { get; set; }
        public string Lock { get; internal set; }
        public string LockFailureReason { get; internal set; }
        public string ValidRelativeTarget { get; internal set; }

        public override HttpResponseMessage ToHttpResponse()
        {
            var httpResponseMessage = base.ToHttpResponse();
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.LOCK, Lock);
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.LOCK_FAILURE_REASON, LockFailureReason);
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.VALID_RELATIVE_TARGET, ValidRelativeTarget);
        
            // Only serialize reponse on success
            if (StatusCode == HttpStatusCode.OK)
            {
                string jsonString = JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                httpResponseMessage.Content = new StringContent(jsonString);
            }
            return httpResponseMessage;
        }
    }
}
