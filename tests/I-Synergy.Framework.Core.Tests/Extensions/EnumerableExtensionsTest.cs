using ISynergy.Framework.Tests.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Extensions.Tests
{
    public class EnumerableExtensionsTest : UnitTest
    {
        [Fact]
        public void NullEnumerableNonFailableTest()
        {
            IEnumerable<object> list = null;
            bool result = false;

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