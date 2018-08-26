using ISynergy.Library;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Extensions.Tests
{
    public class ArrayExtensionsTests
    {
        [Fact]
        [Trait(nameof(ArrayExtensionsTests), Test.Unit)]
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
        [Trait(nameof(ArrayExtensionsTests), Test.Unit)]
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
