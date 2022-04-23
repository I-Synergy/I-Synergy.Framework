using System.Text.Json;
using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class DeleteFileResponse : WopiResponse
    {
        internal DeleteFileResponse()
        { }
        [JsonProperty]
       
        public string Lock { get; internal set; }
        public string LockFailureReason { get; set; }
        
        public override HttpResponseMessage ToHttpResponse()
        {
            var httpResponseMessage = base.ToHttpResponse();
         
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.LOCK, Lock);
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.LOCK_FAILURE_REASON, LockFailureReason);
       
            // Only serialize reponse on success
            if (StatusCode == HttpStatusCode.OK)
            {
                string jsonString = JsonSerializer.Serialize(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                httpResponseMessage.Content = new StringContent(jsonString);
            }
            return httpResponseMessage;
        }
    }
}
