using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;
using Xunit.Abstractions;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Locking
{
    public class SQLiteLockShareModeTests : LockShareModeTests<SQLiteLockServices>
    {
        public SQLiteLockShareModeTests(SQLiteLockServices services, ITestOutputHelper output)
            : base(services, output)
        {
        }
    }
}
