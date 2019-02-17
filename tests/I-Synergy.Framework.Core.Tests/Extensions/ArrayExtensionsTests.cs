using ISynergy.Framework.Tests.Base;
using System;
using System.Collections;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Extensions.Tests
{
    public class ArrayExtensionsTests : UnitTest
    {
        [Fact]
        public void NullArrayNonFailableTest()
        {
            object[] list = null;
            bool result = false;

            foreach (var item in list.EnsureNotNull())
            {
            }

            result = true;

            Assert.True(result);
        }

        [Fact]
        public void NullArrayFailableTest()
        {
            Assert.ThrowsAsync<NullReferenceException>(() =>
            {
                object[] list = null;

                foreach (var item in list)
                {
                }

                return Task.CompletedTask;
            });
        }
    }
}
