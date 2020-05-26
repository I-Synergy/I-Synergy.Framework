using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.FileSystem
{
    public class InMemoryFileSystemTreeCollection : FileSystemTreeCollection<InMemoryFileSystemServices>
    {
        public InMemoryFileSystemTreeCollection(InMemoryFileSystemServices fsServices)
            : base(fsServices)
        {
        }
    }
}
