using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.FileSystem
{
    public class DotNetFileSystemTreeCollection : FileSystemTreeCollection<DotNetFileSystemServices>
    {
        public DotNetFileSystemTreeCollection(DotNetFileSystemServices fsServices)
            : base(fsServices)
        {
        }
    }
}
