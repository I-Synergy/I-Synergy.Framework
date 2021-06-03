using System.Collections.Generic;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ISynergy.Framework.Core.Linq.Exceptions;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class LinqExtensionsTest.
    /// </summary>
    [TestClass]
    public class LinqExtensionsTest
    {
        /// <summary>
        /// Creates the test set.
        /// </summary>
        /// <returns>List&lt;Product&gt;.</returns>
        private List<Product> CreateTestSet()
        {
            var list = new List<Product>
            {
                new Product { ProductId = 1, Name = "Test4", Price = 1m },
                new Product { ProductId = 2, Name = "Test3", Price = 2m },
                new Product { ProductId = 3, Name = "Test3", Price = 3m },
                new Product { ProductId = 4, Name = "Test2", Price = 4m },
                new Product { ProductId = 5, Name = "Test1", Price = 5m }
            };

            return list;
        }


        /// <summary>
        /// Defines the test method StringArrayToOrderByTest.
        /// </summary>
        [TestMethod]
        public void StringArrayToOrderByTest()
        {
            var array = new List<string> { "Name", "ProductId" };
            var data_sorted = CreateTestSet().AsQueryable().OrderBy(string.Join(", ", array)).ToList();

            Assert.IsTrue(data_sorted.First().Name == "Test1");
            Assert.IsTrue(data_sorted.Last().Name == "Test4");
            Assert.IsTrue(data_sorted[2].Name == "Test3" && data_sorted[2].ProductId == 2);
            Assert.IsTrue(data_sorted[3].Name == "Test3" && data_sorted[3].ProductId == 3);
        }

        /// <summary>
        /// Defines the test method StringArrayToOrderDescendingByTest.
        /// </summary>
        [TestMethod]
        public void StringArrayToOrderDescendingByTest()
        {
            var array = new List<string> { "Name", "ProductId" };
            var data_sorted = CreateTestSet().AsQueryable().OrderBy(string.Join(" DESC, ", array)).ToList();

            //Debug.WriteLine(JsonSerializer.Serialize(data_sorted));

            Assert.IsTrue(data_sorted.First().Name == "Test4");
            Assert.IsTrue(data_sorted.Last().Name == "Test1");
            Assert.IsTrue(data_sorted[1].Name == "Test3" && data_sorted[1].ProductId == 2);
            Assert.IsTrue(data_sorted[2].Name == "Test3" && data_sorted[2].ProductId == 3);
        }

        /// <summary>
        /// Defines the test method StringArrayWithInvalidPropertiesToOrderByTest.
        /// </summary>
        [TestMethod]
        public void StringArrayWithInvalidPropertiesToOrderByTest()
        {
            var array = new List<string> { "Name", "AAA", "ProductId", "Test" };
            Assert.ThrowsException<ParseException>(() => CreateTestSet().AsQueryable().OrderBy(string.Join(", ", array)).ToList());
        }
    }
}
