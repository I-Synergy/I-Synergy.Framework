using ISynergy.Framework.Tests.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Extensions.Tests
{
    public class ListExtensionsTests : UnitTest
    {
        [Fact]
        public void NullListNonFailableTest()
        {
            List<object> list = null;
            bool result = false;

            foreach (var item in list.EnsureNotNull())
            {
            }

            result = true;

            Assert.True(result);
        }

        [Fact]
        public void NullListFailableTest()
        {
            Assert.ThrowsAsync<NullReferenceException>(() =>
            {
                List<object> list = null;

                foreach (var item in list)
                {
                }

                return Task.CompletedTask;
            });
        }
    }
}
