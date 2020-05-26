using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    public class EnumerableExtensionsTest
    {
        [Fact]
        public void NullEnumerableNonFailableTest()
        {
            IEnumerable<object> list = null;
            var result = false;

            foreach (var item in list.EnsureNotNull())
            {
            }

            result = true;

            Assert.True(result);
        }

        [Fact]
        public void NullEnumerableFailableTest()
        {
            Assert.ThrowsAsync<NullReferenceException>(() =>
            {
                IEnumerable<object> list = null;

                foreach (var item in list)
                {
                }

                return Task.CompletedTask;
            });
        }
    }
}
