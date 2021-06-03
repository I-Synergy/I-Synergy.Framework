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
        /// Defines the test method LongCount.
        /// </summary>
        [TestMethod]
        public void LongCount()
        {
            //Arrange
            IQueryable testListFull = User.GenerateSampleModels(100).AsQueryable();
            IQueryable testListOne = User.GenerateSampleModels(1).AsQueryable();
            IQueryable testListNone = User.GenerateSampleModels(0).AsQueryable();

            //Act
            var resultFull = testListFull.LongCount();
            var resultOne = testListOne.LongCount();
            var resultNone = testListNone.LongCount();

            //Assert
            Assert.AreEqual(100, resultFull);
            Assert.AreEqual(1, resultOne);
            Assert.AreEqual(0, resultNone);
        }

        /// <summary>
        /// Defines the test method LongCount_Predicate.
        /// </summary>
        [TestMethod]
        public void LongCount_Predicate()
        {
            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            long expected = queryable.LongCount(u => u.Income > 50);
            long result = queryable.LongCount("Income > 50");

            //Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method LongCount_Predicate_WithArgs.
        /// </summary>
        [TestMethod]
        public void LongCount_Predicate_WithArgs()
        {
            const int value = 50;

            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            long expected = queryable.LongCount(u => u.Income > value);
            long result = queryable.LongCount("Income > @0", value);

            //Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method LongCount_Dynamic_Select.
        /// </summary>
        //[TestMethod]
        public void LongCount_Dynamic_Select()
        {
            // Arrange
            IQueryable<User> queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var expected = queryable.Select(x => x.Roles.LongCount()).ToArray();
            var result = queryable.Select("Roles.LongCount()").ToDynamicArray<long>();

            // Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method LongCount_Dynamic_Where.
        /// </summary>
        //[TestMethod]
        public void LongCount_Dynamic_Where()
        {
            const string search = "e";

            // Arrange
            var testList = User.GenerateSampleModels(10);
            var queryable = testList.AsQueryable();

            // Act
            var expected = queryable.Where(u => u.Roles.LongCount(r => r.Name.Contains(search)) > 0).ToArray();
            var result = queryable.Where("Roles.LongCount(Name.Contains(@0)) > 0", search).ToArray();

            Assert.AreEqual(expected, result);
        }
    }
}
