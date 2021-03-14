using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Framework.EntityFramework.Extensions.Tests
{
    /// <summary>
    /// Class QueryableExtensionTests.
    /// </summary>
    public class QueryableExtensionTests
    {
        /// <summary>
        /// Defines the test method GetPageCountTest.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pages">The pages.</param>
        [Theory]
        [InlineData(35, 10, 4)]
        [InlineData(20, 10, 2)]
        [InlineData(8, 10, 1)]
        public void GetPageCountTest(int size, int pageSize, int pages)
        {
            var list = Enumerable.Repeat(new object(), size).AsQueryable();
            Assert.Equal(pages, list.CountPages(pageSize));
        }

        /// <summary>
        /// Defines the test method GetPageTest.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="count">The count.</param>
        [Theory]
        [InlineData(35, 10, 1, 10)]
        [InlineData(35, 10, 2, 10)]
        [InlineData(35, 10, 3, 10)]
        [InlineData(35, 10, 4, 5)]
        [InlineData(20, 10, 2, 10)]
        [InlineData(8, 10, 1, 8)]
        public void GetPageTest(int size, int pageSize, int page, int count)
        {
            var list = Enumerable.Repeat(new object(), size).AsQueryable();
            Assert.Equal(count, list.ToPage(page, pageSize).Count());
        }
    }
}
