using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Synchronization.Client.Messages.Base;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Client.Messages
{
    [DataContract(Name = "changesreq"), Serializable]
    public class HttpMessageSendChangesRequest : BaseHttpMessage
    {
        public HttpMessageSendChangesRequest()
            : base() { }

        public HttpMessageSendChangesRequest(SyncContext context, ScopeInfo scope)
            : base(context)
        {
            Scope = scope;
        }

        /// <summary>
        /// Gets or Sets the reference scope for local repository, stored on server
        /// </summary>
        [DataMember(Name = "scope", IsRequired = true)]
        public ScopeInfo Scope { get; set; }

        /// <summary>
        /// Get the current batch index 
        /// </summary>
        [DataMember(Name = "bi", IsRequired = true)]
        public int BatchIndex { get; set; }

        /// <summary>
        /// Get the current batch count
        /// </summary>
        [DataMember(Name = "bc", IsRequired = false)]
        public int BatchCount { get; set; }

        /// <summary>
        /// Gets or Sets if this is the last Batch to sent to server 
        /// </summary>
        [DataMember(Name = "islb", IsRequired = true)]
        public bool IsLastBatch { get; set; }

        /// <summary>
        /// Changes to send
        /// </summary>
        [DataMember(Name = "changes", IsRequired = true)]
        public ContainerSet Changes { get; set; }
    }
}
