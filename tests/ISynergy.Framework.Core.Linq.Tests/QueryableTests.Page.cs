using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        //[TestMethod]
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
            Assert.AreEqual(testList.Skip((page - 1) * pageSize).Take(pageSize).ToArray(), result.ToDynamicArray<User>());
        }

        /// <summary>
        /// Defines the test method Page_TSource.
        /// </summary>
        //[TestMethod]
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
            Assert.AreEqual(testList.Skip((page - 1) * pageSize).Take(pageSize).ToArray(), result.ToDynamicArray<User>());
        }

        /// <summary>
        /// Defines the test method PageResult.
        /// </summary>
        //[TestMethod]
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
            Assert.AreEqual(page, result.CurrentPage);
            Assert.AreEqual(pageSize, result.PageSize);
            Assert.AreEqual(total, result.RowCount);
            Assert.AreEqual(5, result.PageCount);
            Assert.AreEqual(testList.Skip((page - 1) * pageSize).Take(pageSize).ToArray(), result.Queryable.ToDynamicArray<User>());
        }

        /// <summary>
        /// Defines the test method PageResult_TSource.
        /// </summary>
        //[TestMethod]
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
            Assert.AreEqual(page, result.CurrentPage);
            Assert.AreEqual(pageSize, result.PageSize);
            Assert.AreEqual(total, result.RowCount);
            Assert.AreEqual(5, result.PageCount);
            Assert.AreEqual(testList.Skip((page - 1) * pageSize).Take(pageSize).ToArray(), result.Queryable.ToDynamicArray<User>());
        }
    }
}
