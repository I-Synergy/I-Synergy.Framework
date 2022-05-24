using ISynergy.Framework.Core.Data.Tests.TestClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class EnumerableExtensionsTest.
    /// </summary>
    [TestClass]
    public class EnumerableExtensionsTest
    {
        /// <summary>
        /// Defines the test method NullEnumerableNonFailableTest.
        /// </summary>
        [TestMethod]
        public void NullEnumerableNonFailableTest()
        {
            IEnumerable<object> list = null;
            var result = false;

            foreach (var item in list.EnsureNotNull())
            {
            }

            result = true;

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Defines the test method NullEnumerableFailableTest.
        /// </summary>
        [TestMethod]
        public void NullEnumerableFailableTest()
        {
            Assert.ThrowsExceptionAsync<NullReferenceException>(() =>
            {
                IEnumerable<object> list = null;

                foreach (var item in list)
                {
                }

                return Task.CompletedTask;
            });
        }

        [TestMethod]
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

        [TestMethod]
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
