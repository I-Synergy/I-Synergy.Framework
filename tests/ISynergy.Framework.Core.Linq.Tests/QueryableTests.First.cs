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
        /// Defines the test method First.
        /// </summary>
        [TestMethod]
        public void First()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.First();

            //Assert
            Assert.AreEqual(testList[0].Id, result.Id);
        }

        /// <summary>
        /// Defines the test method First_Predicate.
        /// </summary>
        [TestMethod]
        public void First_Predicate()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            //Act
            var expected = queryable.First(u => u.Income > 1000);
            var result = queryable.First("Income > 1000");

            //Assert
            Assert.AreEqual(expected as object, result);
        }

        /// <summary>
        /// Defines the test method First_Predicate_WithArgs.
        /// </summary>
        [TestMethod]
        public void First_Predicate_WithArgs()
        {
            const int value = 1000;

            //Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            //Act
            var expected = queryable.First(u => u.Income > value);
            var result = queryable.First("Income > @0", value);

            //Assert
            Assert.AreEqual(expected as object, result);
        }

        /// <summary>
        /// Defines the test method First_Dynamic.
        /// </summary>
        //[TestMethod]
        public void First_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var realResult = testList.OrderBy(x => x.Roles.First().Name).Select(x => x.Id).ToArray();
            var testResult = testListQry.OrderBy("Roles.First().Name").Select("Id");

            //Assert
            Assert.AreEqual(realResult, testResult.Cast<Guid>().ToArray());
        }
    }
}
