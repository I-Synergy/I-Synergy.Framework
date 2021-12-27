using ISynergy.Framework.Synchronization.Client.Messages.Base;
using ISynergy.Framework.Synchronization.Core;
using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Client.Messages
{
    [DataContract(Name = "ensurereq"), Serializable]
    public class HttpMessageEnsureScopesRequest : BaseHttpMessage
    {
        public HttpMessageEnsureScopesRequest() 
            : base() { }

        /// <summary>
        /// Create a new message to web remote server.
        /// Scope info table name is not provided since we do not care about it on the server side
        /// </summary>
        public HttpMessageEnsureScopesRequest(SyncContext context) 
            : base(context) { }
    }
}
