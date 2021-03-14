using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class ListExtensionsTests.
    /// </summary>
    public class ListExtensionsTests
    {
        /// <summary>
        /// Defines the test method NullListNonFailableTest.
        /// </summary>
        [Fact]
        public void NullListNonFailableTest()
        {
            List<object> list = null;
            var result = false;

            foreach (var item in list.EnsureNotNull())
            {
            }

            result = true;

            Assert.True(result);
        }

        /// <summary>
        /// Defines the test method NullListFailableTest.
        /// </summary>
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
