using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class RenameFileResponse : WopiResponse
    {
        internal RenameFileResponse()
        { }
        [JsonProperty]
        public string RenamedFileBaseName { get; internal set; }
        public string Lock { get; internal set; }
        public string LockFailureReason { get; set; }
        public string ItemVersion { get; set; }
        public string InvalidFileNameError { get; set; }

        public override HttpResponseMessage ToHttpResponse()
        {
            var httpResponseMessage = base.ToHttpResponse();
         
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.LOCK, Lock);
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.LOCK_FAILURE_REASON, LockFailureReason);
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.ITEM_VERSION, ItemVersion);
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.INVALID_FILE_NAME_ERROR, InvalidFileNameError);


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
