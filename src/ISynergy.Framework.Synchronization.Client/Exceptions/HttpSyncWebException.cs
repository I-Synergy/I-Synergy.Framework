using ISynergy.Framework.Synchronization.Core;
using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Synchronization.Client.Exceptions
{
    /// <summary>
    /// Http sync exception.
    /// </summary>
    public class HttpSyncWebException : SyncException
    {
        /// <summary>
        /// Gets or sets the reason phrase which typically is sent by servers together with the status code.
        /// </summary>
        public string ReasonPhrase { get; }

        /// <summary>
        /// Gets or sets the status code of the HTTP response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// default constructor.
        /// </summary>
        /// <param name="message"></param>
        public HttpSyncWebException(string message) : base(message)
        {
        }

        /// <summary>
        /// default constructor.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="reasonPhrase"></param>
        /// <param name="httpStatus"></param>
        public HttpSyncWebException(string message, string reasonPhrase, HttpStatusCode httpStatus)
            : this(message)
        {
            ReasonPhrase = reasonPhrase;
            StatusCode = httpStatus;
        }

        /// <summary>
        /// default constructor.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="response"></param>
        public HttpSyncWebException(string message, HttpResponseMessage response)
            : this(message, response.ReasonPhrase, response.StatusCode)
        {
        }
    }
}
