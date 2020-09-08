using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.FileSystem
{
    public class InMemorySimpleFsTests : SimpleFsTests<InMemoryFileSystemServices>
    {
        public InMemorySimpleFsTests(InMemoryFileSystemServices fsServices)
            : base(fsServices)
        {
        }
    }
}
