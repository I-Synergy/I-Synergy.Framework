using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    public class PutUserInfoRequest : WopiRequest
    {
        internal PutUserInfoRequest(HttpRequestMessage httpRequestMessage, string fileId) : base(httpRequestMessage, fileId)
        {
            // can't have async constructors, so just block since userinfo is only 1024 ASCII chars
            UserInfo = httpRequestMessage.Content.ReadAsStringAsync().Result;

            //var stream = putUserInfoRequest.InputStream;
            //var bytes = new byte[stream.Length];
            //await stream.ReadAsync(bytes, 0, (int)stream.Length);
            //file.UserInfo = System.Text.Encoding.UTF8.GetString(bytes);
        }

        public HttpContent Content { get; private set; }
        public string UserInfo { get; private set; }

        public PutUserInfoResponse ResponseOK()
        {
            return new PutUserInfoResponse()
            {
                StatusCode = HttpStatusCode.OK
            };

        }

        public WopiResponse ResponseNotImplemented()
        {
            return new PutUserInfoResponse()
            {
                StatusCode = HttpStatusCode.NotImplemented
            };
        }
    }
}
