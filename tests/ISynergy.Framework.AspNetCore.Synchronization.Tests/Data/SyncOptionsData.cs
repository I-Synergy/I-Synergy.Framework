using ISynergy.Framework.Synchronization.Core;
using System.Collections;
using System.Collections.Generic;

namespace ISynergy.Framework.AspNetCore.Synchronization.Tests.Data
{
    public class SyncOptionsData : IEnumerable<object[]>
    {

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new SyncOptions { BatchSize = 50 } };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
