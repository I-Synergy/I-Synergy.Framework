using System;
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
        /// Defines the test method Last.
        /// </summary>
        [TestMethod]
        public void Last()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var realResult = testList.Last();
            var result = testListQry.Last();

            //Assert
            Assert.AreEqual(realResult.Id, result.Id);
        }

        /// <summary>
        /// Defines the test method Last_Predicate.
        /// </summary>
        [TestMethod]
        public void Last_Predicate()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            //Act
            var expected = queryable.Last(u => u.Income > 1000);
            var result = queryable.Last("Income > 1000");

            //Assert
            Assert.AreEqual(expected as object, result);
        }

        /// <summary>
        /// Defines the test method LastOrDefault.
        /// </summary>
        [TestMethod]
        public void LastOrDefault()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var realResult = testList.LastOrDefault();
            var singleResult = testListQry.LastOrDefault();
            var defaultResult = Enumerable.Empty<User>().AsQueryable().FirstOrDefault();

            //Assert
            Assert.AreEqual(realResult.Id, singleResult.Id);
            Assert.IsNull(defaultResult);
        }

        /// <summary>
        /// Defines the test method LastOrDefault_Predicate.
        /// </summary>
        [TestMethod]
        public void LastOrDefault_Predicate()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            //Act
            var expected = queryable.LastOrDefault(u => u.Income > 1000);
            var result = queryable.LastOrDefault("Income > 1000");

            //Assert
            Assert.AreEqual(expected as object, result);
        }

        /// <summary>
        /// Defines the test method Last_Dynamic.
        /// </summary>
        //[TestMethod]
        public void Last_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var realResult = testList.OrderBy(x => x.Roles.Last().Name).Select(x => x.Id).ToArray();
            var testResult = testListQry.OrderBy("Roles.Last().Name").Select("Id");

            //Assert
            Assert.AreEqual(realResult, testResult.Cast<Guid>().ToArray());
        }
    }
}
