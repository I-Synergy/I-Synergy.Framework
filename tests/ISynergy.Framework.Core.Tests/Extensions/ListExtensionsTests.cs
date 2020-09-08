using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    public class ListExtensionsTests
    {
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
