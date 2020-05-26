using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.FileSystem
{
    public class SQLiteFileSystemTreeCollection : FileSystemTreeCollection<SQLiteFileSystemServices>
    {
        public SQLiteFileSystemTreeCollection(SQLiteFileSystemServices fsServices)
            : base(fsServices)
        {
        }
    }
}
