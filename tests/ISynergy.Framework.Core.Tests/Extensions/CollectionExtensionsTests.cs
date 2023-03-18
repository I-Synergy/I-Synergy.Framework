using ISynergy.Framework.Core.Collections;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class CollectionExtensionsTests.
    /// </summary>
    [TestClass]
    public class CollectionExtensionsTests
    {
        /// <summary>
        /// Defines the test method NullObservableCollectionNonFailableTest.
        /// </summary>
        [TestMethod]
        public void NullObservableCollectionNonFailableTest()
        {
            ObservableCollection<object> list = null;
            var result = false;

            foreach (var item in list.EnsureNotNull()) { }

            result = true;

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Defines the test method NullObservableCollectionFailableTest.
        /// </summary>
        [TestMethod]
        public void NullObservableCollectionFailableTest()
        {
            Assert.ThrowsExceptionAsync<NullReferenceException>(() =>
            {
                ObservableCollection<object> list = null;

                foreach (var item in list) { }

                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// ICollectionTToDataTable Test
        /// </summary>
        [TestMethod]
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

                Assert.IsNotNull(dataTable);
                Assert.AreEqual(5, dataTable.Rows.Count);
                Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.ProductGroups)));
                Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.Properties)));
                Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.Errors)));
                Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.Validator)));
                Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.IsValid)));
                Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.IsDirty)));
            }
            else
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// ICollectionToDataTable Test
        /// </summary>
        [TestMethod]
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

                Assert.IsNotNull(dataTable);
                Assert.AreEqual(5, dataTable.Rows.Count);
                Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.ProductGroups)));
                Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.Properties)));
                Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.Errors)));
                Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.Validator)));
                Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.IsValid)));
                Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.IsDirty)));
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
