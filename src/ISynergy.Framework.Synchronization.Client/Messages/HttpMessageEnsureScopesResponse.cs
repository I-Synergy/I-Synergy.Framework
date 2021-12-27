using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Synchronization.Client.Messages.Base;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Scopes;
using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Client.Messages
{
    [DataContract(Name = "ensurescopesres"), Serializable]
    public class HttpMessageEnsureScopesResponse : BaseHttpMessage
    {
        public HttpMessageEnsureScopesResponse() : base() { }

        public HttpMessageEnsureScopesResponse(SyncContext context, ServerScopeInfo serverScopeInfo)
            : base(context)
        {
            Argument.IsNotNull(serverScopeInfo);

            ServerScopeInfo = serverScopeInfo;
        }

        /// <summary>
        /// Gets or Sets the schema option (without schema itself, that is not serializable)
        /// </summary>
        [DataMember(Name = "serverscope", IsRequired = true)]
        public ServerScopeInfo ServerScopeInfo { get; set; }
    }
}
