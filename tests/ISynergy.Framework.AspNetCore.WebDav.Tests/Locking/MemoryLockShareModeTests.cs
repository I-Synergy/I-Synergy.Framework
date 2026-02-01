using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;
using Xunit.Abstractions;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Locking
{
    public class MemoryLockShareModeTests : LockShareModeTests<MemoryLockServices>
    {
        public MemoryLockShareModeTests(MemoryLockServices services, ITestOutputHelper output)
            : base(services, output)
        {
        }
    }
}
