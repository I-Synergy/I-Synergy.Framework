using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.FileSystem
{
    public class SQLiteSimpleFsTests : SimpleFsTests<SQLiteFileSystemServices>
    {
        public SQLiteSimpleFsTests(SQLiteFileSystemServices fsServices)
            : base(fsServices)
        {
        }
    }
}
