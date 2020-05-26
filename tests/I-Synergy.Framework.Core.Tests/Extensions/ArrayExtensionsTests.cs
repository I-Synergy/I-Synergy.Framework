using System;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    public class ArrayExtensionsTests
    {
        [Fact]
        public void NullArrayNonFailableTest()
        {
            object[] list = null;
            var result = false;

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
