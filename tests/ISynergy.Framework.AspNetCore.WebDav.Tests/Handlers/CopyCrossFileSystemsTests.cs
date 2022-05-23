using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Handlers
{
    public class CopyCrossFileSystemsTests : CopyTestsBase
    {
        public CopyCrossFileSystemsTests()
            : base(RecursiveProcessingMode.PreferCrossFileSystem, GetETagProperty.PropertyName)
        {
        }
    }
}
