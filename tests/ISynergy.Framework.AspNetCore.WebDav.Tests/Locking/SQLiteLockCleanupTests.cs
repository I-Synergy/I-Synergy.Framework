using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Locking
{
    public class SQLiteLockCleanupTests : LockCleanupTests<SQLiteLockServices>
    {
        public SQLiteLockCleanupTests(SQLiteLockServices services)
            : base(services)
        {
        }
    }
}
