using ISynergy.Framework.Synchronization.Core;
using System.Net;

namespace ISynergy.Framework.Synchronization.Client.Exceptions
{
    public class HttpSyncWebException : SyncException
    {
        /// <summary>
        /// Gets or sets the reason phrase which typically is sent by servers together with the status code.
        /// </summary>
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// Gets or sets the status code of the HTTP response.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        public HttpSyncWebException(string message) : base(message)
        {
        }


    }
}
