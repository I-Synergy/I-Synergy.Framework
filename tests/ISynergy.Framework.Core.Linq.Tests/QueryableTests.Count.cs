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
        /// Defines the test method Count.
        /// </summary>
        [TestMethod]
        public void Count()
        {
            //Arrange
            IQueryable testListFull = User.GenerateSampleModels(100).AsQueryable();
            IQueryable testListOne = User.GenerateSampleModels(1).AsQueryable();
            IQueryable testListNone = User.GenerateSampleModels(0).AsQueryable();

            //Act
            var resultFull = testListFull.Count();
            var resultOne = testListOne.Count();
            var resultNone = testListNone.Count();

            //Assert
            Assert.AreEqual(100, resultFull);
            Assert.AreEqual(1, resultOne);
            Assert.AreEqual(0, resultNone);
        }

        /// <summary>
        /// Defines the test method Count_Predicate.
        /// </summary>
        [TestMethod]
        public void Count_Predicate()
        {
            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            int expected = queryable.Count(u => u.Income > 50);
            int result = queryable.Count("Income > 50");

            //Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method Count_Predicate_WithArgs.
        /// </summary>
        [TestMethod]
        public void Count_Predicate_WithArgs()
        {
            const int value = 50;

            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            int expected = queryable.Count(u => u.Income > value);
            int result = queryable.Count("Income > @0", value);

            //Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method Count_Dynamic_Select.
        /// </summary>
        //[TestMethod]
        public void Count_Dynamic_Select()
        {
            // Arrange
            IQueryable<User> queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var expected = queryable.Select(x => x.Roles.Count()).ToArray();
            var result = queryable.Select("Roles.Count()").ToDynamicArray<int>();

            // Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method Count_Dynamic_Where.
        /// </summary>
        //[TestMethod]
        public void Count_Dynamic_Where()
        {
            const string search = "e";

            // Arrange
            var testList = User.GenerateSampleModels(10);
            var queryable = testList.AsQueryable();

            // Act
            var expected = queryable.Where(u => u.Roles.Count(r => r.Name.Contains(search)) > 0).ToArray();
            var result = queryable.Where("Roles.Count(Name.Contains(@0)) > 0", search).ToArray();

            Assert.AreEqual(expected, result);
        }
    }
}
