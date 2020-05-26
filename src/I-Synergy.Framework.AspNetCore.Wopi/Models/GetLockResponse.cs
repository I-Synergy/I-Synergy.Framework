using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class GetLockResponse : WopiResponse
    {
        internal GetLockResponse()
        { }

        public string Lock { get; internal set; }
       
        public string LockFailureReason { get; set; }

        public override HttpResponseMessage ToHttpResponse()
        {
            var httpResponseMessage = base.ToHttpResponse();
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.LOCK, Lock);
            SetHttpResponseHeader(httpResponseMessage, WopiResponseHeaders.LOCK_FAILURE_REASON, LockFailureReason);
            return httpResponseMessage;
        }
    }
      
}