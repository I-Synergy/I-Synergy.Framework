using ISynergy.Framework.Core.Data.Tests.TestClasses;
using System;
using System.Collections;
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

        [Fact]
        public void IEnumerableTToDataTableTest()
        {
            var collection = new List<Product>()
            {
                new Product{ ProductId = Guid.NewGuid(), Name ="Test1" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test2" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test3" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test4" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test5" }
            };

            if (collection is IEnumerable<Product> data)
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

        [Fact]
        public void IEnumerableToDataTableTest()
        {
            var collection = new List<Product>()
            {
                new Product{ ProductId = Guid.NewGuid(), Name ="Test1" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test2" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test3" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test4" },
                new Product{ ProductId = Guid.NewGuid(), Name ="Test5" }
            };

            if (collection is IEnumerable data)
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
