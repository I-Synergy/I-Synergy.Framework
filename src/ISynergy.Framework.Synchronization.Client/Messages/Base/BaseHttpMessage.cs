using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Synchronization.Core;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Client.Messages.Base
{
    [DataContract]
    public abstract class BaseHttpMessage
    {
        [DataMember(Name = "sc", IsRequired = true)]
        public SyncContext SyncContext { get; private set; }

        protected BaseHttpMessage()
        {
        }

        protected BaseHttpMessage(SyncContext context)
        {
            Argument.IsNotNull(context);
            SyncContext = context;
        }
    }
}
