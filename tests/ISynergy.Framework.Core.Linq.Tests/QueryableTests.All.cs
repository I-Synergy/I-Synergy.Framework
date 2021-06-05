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
        /// Defines the test method All_Predicate.
        /// </summary>
        [TestMethod]
        public void All_Predicate()
        {
            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            bool expected = queryable.All(u => u.Income > 50);
            bool result = queryable.All("Income > 50");

            //Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method All_Predicate_WithArgs.
        /// </summary>
        [TestMethod]
        public void All_Predicate_WithArgs()
        {
            const int value = 50;

            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            bool expected = queryable.All(u => u.Income > value);
            bool result = queryable.All("Income > @0", value);

            //Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method All_Dynamic_Select.
        /// </summary>
        //[TestMethod]
        public void All_Dynamic_Select()
        {
            // Arrange
            IQueryable<User> queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var expected = queryable.Select(x => x.Roles.All(r => r.Name != null)).ToArray();
            var result = queryable.Select("Roles.All(Name != null)").ToDynamicArray<bool>();

            // Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method All_Dynamic_Where.
        /// </summary>
        [TestMethod]
        public void All_Dynamic_Where()
        {
            const string search = "e";

            // Arrange
            var testList = User.GenerateSampleModels(10);
            var queryable = testList.AsQueryable();

            // Act
            var expected = queryable.Where(u => u.Roles.All(r => r.Name.Contains(search))).ToArray();
            var result = queryable.Where("Roles.All(Name.Contains(@0))", search).ToArray();

            Assert.AreEqual(expected, result);
        }
    }
}
