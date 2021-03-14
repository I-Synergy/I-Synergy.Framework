using System;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class ArrayExtensionsTests.
    /// </summary>
    public class ArrayExtensionsTests
    {
        /// <summary>
        /// Defines the test method NullArrayNonFailableTest.
        /// </summary>
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

        /// <summary>
        /// Defines the test method NullArrayFailableTest.
        /// </summary>
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
