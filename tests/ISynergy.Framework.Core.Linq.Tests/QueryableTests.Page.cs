using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method Page.
        /// </summary>
        [Fact]
        public void Page()
        {
            //Arrange
            const int total = 35;
            const int page = 2;
            const int pageSize = 10;
            var testList = User.GenerateSampleModels(total);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.Page(page, pageSize);

            //Assert
            Assert.Equal(testList.Skip((page - 1) * pageSize).Take(pageSize).ToArray(), result.ToDynamicArray<User>());
        }

        /// <summary>
        /// Defines the test method Page_TSource.
        /// </summary>
        [Fact]
        public void Page_TSource()
        {
            //Arrange
            const int total = 35;
            const int page = 2;
            const int pageSize = 10;
            var testList = User.GenerateSampleModels(total);
            var testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.Page(page, pageSize);

            //Assert
            Assert.Equal(testList.Skip((page - 1) * pageSize).Take(pageSize).ToArray(), result.ToDynamicArray<User>());
        }

        /// <summary>
        /// Defines the test method PageResult.
        /// </summary>
        [Fact]
        public void PageResult()
        {
            //Arrange
            const int total = 44;
            const int page = 2;
            const int pageSize = 10;
            var testList = User.GenerateSampleModels(total);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.PageResult(page, pageSize);

            //Assert
            Assert.Equal(page, result.CurrentPage);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(total, result.RowCount);
            Assert.Equal(5, result.PageCount);
            Assert.Equal(testList.Skip((page - 1) * pageSize).Take(pageSize).ToArray(), result.Queryable.ToDynamicArray<User>());
        }

        /// <summary>
        /// Defines the test method PageResult_TSource.
        /// </summary>
        [Fact]
        public void PageResult_TSource()
        {
            //Arrange
            const int total = 44;
            const int page = 2;
            const int pageSize = 10;
            var testList = User.GenerateSampleModels(total);
            var testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.PageResult(page, pageSize);

            //Assert
            Assert.Equal(page, result.CurrentPage);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(total, result.RowCount);
            Assert.Equal(5, result.PageCount);
            Assert.Equal(testList.Skip((page - 1) * pageSize).Take(pageSize).ToArray(), result.Queryable.ToDynamicArray<User>());
        }
    }
}
