using ISynergy.Framework.Core.Data.Tests.TestClasses;
using System;
using System.Collections;
using System.Collections.Generic;
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

        /// <summary>
        /// ICollectionTToDataTable Test
        /// </summary>
        [Fact]
        public void ICollectionTToDataTableTest()
        {
            var collection = new List<Product>()
            {
                new Product{ ProductId = Guid.NewGuid(), Name ="Test1" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test2" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test3" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test4" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test5" }
            };

            if(collection is ICollection<Product> data)
            {
                var dataTable = data.ToDataTable<Product>("Test");

                Assert.NotNull(dataTable);
                Assert.Equal(5, dataTable.Rows.Count);
                Assert.False(dataTable.Columns.Contains(nameof(Product.ProductGroups)));
                Assert.False(dataTable.Columns.Contains(nameof(Product.Properties)));
                Assert.False(dataTable.Columns.Contains(nameof(Product.Errors)));
                Assert.False(dataTable.Columns.Contains(nameof(Product.Validator)));
                Assert.False(dataTable.Columns.Contains(nameof(Product.IsValid)));
                Assert.False(dataTable.Columns.Contains(nameof(Product.IsDirty)));
            }
            else
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// ICollectionToDataTable Test
        /// </summary>
        [Fact]
        public void ICollectionToDataTableTest()
        {
            var collection = new List<Product>()
            {
                new Product{ ProductId = Guid.NewGuid(), Name ="Test1" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test2" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test3" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test4" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test5" }
            };

            if (collection is ICollection data)
            {
                var dataTable = data.ToDataTable(typeof(Product), "Test");

                Assert.NotNull(dataTable);
                Assert.Equal(5, dataTable.Rows.Count);
                Assert.False(dataTable.Columns.Contains(nameof(Product.ProductGroups)));
                Assert.False(dataTable.Columns.Contains(nameof(Product.Properties)));
                Assert.False(dataTable.Columns.Contains(nameof(Product.Errors)));
                Assert.False(dataTable.Columns.Contains(nameof(Product.Validator)));
                Assert.False(dataTable.Columns.Contains(nameof(Product.IsValid)));
                Assert.False(dataTable.Columns.Contains(nameof(Product.IsDirty)));
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
