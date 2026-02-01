using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Locking
{
    public class MemoryLockCleanupTests : LockCleanupTests<MemoryLockServices>
    {
        public MemoryLockCleanupTests(MemoryLockServices services)
            : base(services)
        {
        }
    }
}
