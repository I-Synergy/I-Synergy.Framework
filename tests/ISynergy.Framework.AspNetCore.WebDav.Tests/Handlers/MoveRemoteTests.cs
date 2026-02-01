using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Handlers
{
    public class MoveRemoteTests : MoveTestsBase
    {
        public MoveRemoteTests()
            : base(RecursiveProcessingMode.PreferCrossServer, GetETagProperty.PropertyName)
        {
        }
    }
}
