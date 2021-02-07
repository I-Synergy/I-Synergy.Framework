using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class CollectionExtensionsTests.
    /// </summary>
    public class CollectionExtensionsTests
    {
        /// <summary>
        /// Defines the test method NullObservableCollectionNonFailableTest.
        /// </summary>
        [Fact]
        public void NullObservableCollectionNonFailableTest()
        {
            ObservableCollection<object> list = null;
            var result = false;

            foreach (var item in list.EnsureNotNull()) { }

            result = true;

            Assert.True(result);
        }

        /// <summary>
        /// Defines the test method NullObservableCollectionFailableTest.
        /// </summary>
        [Fact]
        public void NullObservableCollectionFailableTest()
        {
            Assert.ThrowsAsync<NullReferenceException>(() =>
            {
                ObservableCollection<object> list = null;

                foreach (var item in list) { }

                return Task.CompletedTask;
            });
        }
    }
}
