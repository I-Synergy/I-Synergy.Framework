using System.Collections.Generic;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using Xunit;
using System.Linq;
using ISynergy.Framework.Core.Linq.Exceptions;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public class LinqExtensionsTest
    {
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


        [Fact]
        public void StringArrayToOrderByTest()
        {
            var array = new List<string> { "Name", "ProductId" };
            var data_sorted = CreateTestSet().AsQueryable().OrderBy(string.Join(", ", array)).ToList();

            Assert.True(data_sorted.First().Name == "Test1");
            Assert.True(data_sorted.Last().Name == "Test4");
            Assert.True(data_sorted[2].Name == "Test3" && data_sorted[2].ProductId == 2);
            Assert.True(data_sorted[3].Name == "Test3" && data_sorted[3].ProductId == 3);
        }

        [Fact]
        public void StringArrayToOrderDescendingByTest()
        {
            var array = new List<string> { "Name", "ProductId" };
            var data_sorted = CreateTestSet().AsQueryable().OrderBy(string.Join(" DESC, ", array)).ToList();

            //Debug.WriteLine(JsonSerializer.Serialize(data_sorted));

            Assert.True(data_sorted.First().Name == "Test4");
            Assert.True(data_sorted.Last().Name == "Test1");
            Assert.True(data_sorted[1].Name == "Test3" && data_sorted[1].ProductId == 2);
            Assert.True(data_sorted[2].Name == "Test3" && data_sorted[2].ProductId == 3);
        }

        [Fact]
        public void StringArrayWithInvalidPropertiesToOrderByTest()
        {
            var array = new List<string> { "Name", "AAA", "ProductId", "Test" };
            Assert.Throws<ParseException>(() => CreateTestSet().AsQueryable().OrderBy(string.Join(", ", array)).ToList());
        }
    }
}
