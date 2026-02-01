using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.FileSystem
{
    public class DotNetSimpleFsTests : SimpleFsTests<DotNetFileSystemServices>
    {
        public DotNetSimpleFsTests(DotNetFileSystemServices fsServices)
            : base(fsServices)
        {
        }
    }
}
