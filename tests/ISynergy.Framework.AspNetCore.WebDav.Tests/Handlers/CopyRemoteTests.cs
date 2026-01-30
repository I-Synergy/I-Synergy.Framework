using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Handlers
{
    public class CopyRemoteTests : CopyTestsBase
    {
        public CopyRemoteTests()
            : base(RecursiveProcessingMode.PreferCrossServer, GetETagProperty.PropertyName)
        {
        }
    }
}
