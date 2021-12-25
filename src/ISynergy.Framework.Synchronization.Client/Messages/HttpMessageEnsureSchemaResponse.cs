using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Synchronization.Client.Messages.Base;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Client.Messages
{
    [DataContract(Name = "ensureschemares"), Serializable]
    public class HttpMessageEnsureSchemaResponse : BaseHttpMessage  
    {
        public HttpMessageEnsureSchemaResponse()
            : base() { }

        public HttpMessageEnsureSchemaResponse(SyncContext context, ServerScopeInfo serverScopeInfo)
            : base(context)
        {
            Argument.IsNotNull(serverScopeInfo);

            ServerScopeInfo = serverScopeInfo;
            Schema = serverScopeInfo.Schema;
        }

        /// <summary>
        /// Gets or Sets the schema because the ServerScopeInfo won't have it since it's marked (on purpose) as IgnoreDataMember (and then not serialized)
        /// </summary>
        [DataMember(Name = "schema", IsRequired = true)]
        public SyncSet Schema { get; set; }

        /// <summary>
        /// Gets or Sets the server scope info, from server
        /// </summary>
        [DataMember(Name = "ssi", IsRequired = true)]
        public ServerScopeInfo ServerScopeInfo { get; set; }
    }
}
