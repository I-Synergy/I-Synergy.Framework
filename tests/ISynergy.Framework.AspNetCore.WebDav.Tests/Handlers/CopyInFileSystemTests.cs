using ISynergy.Framework.AspNetCore.WebDav.Server;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Handlers
{
    public class CopyInFileSystemTests : CopyTestsBase
    {
        public CopyInFileSystemTests()
            : base(RecursiveProcessingMode.PreferFastest)
        {
        }
    }
}
