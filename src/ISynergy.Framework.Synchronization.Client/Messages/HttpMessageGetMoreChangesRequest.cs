using ISynergy.Framework.Synchronization.Client.Messages.Base;
using ISynergy.Framework.Synchronization.Core;
using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Client.Messages
{
    [DataContract(Name = "morechangesreq"), Serializable]
    public class HttpMessageGetMoreChangesRequest : BaseHttpMessage
    {
        public HttpMessageGetMoreChangesRequest() : base() { }

        public HttpMessageGetMoreChangesRequest(SyncContext context, int batchIndexRequested)
             : base(context)
        {
            BatchIndexRequested = batchIndexRequested;
        }

        [DataMember(Name = "bireq", IsRequired = true)]
        public int BatchIndexRequested { get; set; }
    }
}
