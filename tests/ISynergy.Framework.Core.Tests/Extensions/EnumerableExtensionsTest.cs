using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class EnumerableExtensionsTest.
    /// </summary>
    public class EnumerableExtensionsTest
    {
        /// <summary>
        /// Defines the test method NullEnumerableNonFailableTest.
        /// </summary>
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

        /// <summary>
        /// Defines the test method NullEnumerableFailableTest.
        /// </summary>
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
