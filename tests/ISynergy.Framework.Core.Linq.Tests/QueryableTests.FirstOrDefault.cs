using System;
using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using NFluent;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method FirstOrDefault.
        /// </summary>
        [Fact]
        public void FirstOrDefault()
        {
            // Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            // Act
            var expected = Queryable.FirstOrDefault(queryable);
            var expectedDefault = Queryable.FirstOrDefault(Enumerable.Empty<User>().AsQueryable());

            var result = (queryable as IQueryable).FirstOrDefault();
            var resultDefault = (Enumerable.Empty<User>().AsQueryable() as IQueryable).FirstOrDefault();

            // Assert
            Check.That(result).Equals(expected);
            Assert.Null(expectedDefault);
            Assert.Null(resultDefault);
        }

        /// <summary>
        /// Defines the test method FirstOrDefault_Predicate.
        /// </summary>
        [Fact]
        public void FirstOrDefault_Predicate()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            // Act
            var expected = queryable.FirstOrDefault(u => u.Income > 1000);
            var result = queryable.FirstOrDefault("Income > 1000");

            // Assert
            Check.That(result).Equals(expected);
        }

        /// <summary>
        /// Defines the test method FirstOrDefault_Predicate_WithArgs.
        /// </summary>
        [Fact]
        public void FirstOrDefault_Predicate_WithArgs()
        {
            const int value = 1000;

            // Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            // Act
            var expected = queryable.FirstOrDefault(u => u.Income > value);
            var result = queryable.FirstOrDefault("Income > @0", value);

            // Assert
            Check.That(result).Equals(expected);
        }

        /// <summary>
        /// Defines the test method FirstOrDefault_Dynamic.
        /// </summary>
        [Fact]
        public void FirstOrDefault_Dynamic()
        {
            // Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            // Act
            var realResult = testList.OrderBy(x => x.Roles.First().Name).Select(x => x.Id).ToArray();
            var result = testListQry.OrderBy("Roles.FirstOrDefault().Name").Select("Id");

            // Assert
            Check.That(result.ToDynamicArray().Cast<Guid>()).ContainsExactly(realResult);
        }
    }
}
