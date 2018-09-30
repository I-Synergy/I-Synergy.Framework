using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Extensions.Tests
{
    public class CollectionExtensionsTests
    {
        [Fact]
        [Trait(nameof(CollectionExtensionsTests), Test.Unit)]
        public void NullObservableCollectionNonFailableTest()
        {
            ObservableCollection<object> list = null;
            bool result = false;

            foreach (var item in list.EnsureNotNull()) { }

            result = true;

            Assert.True(result);
        }

        [Fact]
        [Trait(nameof(CollectionExtensionsTests), Test.Unit)]
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
