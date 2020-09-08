using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public partial class QueryableTests
    {
        [Fact]
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
            Assert.Equal(100, resultFull);
            Assert.Equal(1, resultOne);
            Assert.Equal(0, resultNone);
        }

        [Fact]
        public void Count_Predicate()
        {
            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            int expected = queryable.Count(u => u.Income > 50);
            int result = queryable.Count("Income > 50");

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Count_Predicate_WithArgs()
        {
            const int value = 50;

            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            int expected = queryable.Count(u => u.Income > value);
            int result = queryable.Count("Income > @0", value);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Count_Dynamic_Select()
        {
            // Arrange
            IQueryable<User> queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var expected = queryable.Select(x => x.Roles.Count()).ToArray();
            var result = queryable.Select("Roles.Count()").ToDynamicArray<int>();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Count_Dynamic_Where()
        {
            const string search = "e";

            // Arrange
            var testList = User.GenerateSampleModels(10);
            var queryable = testList.AsQueryable();

            // Act
            var expected = queryable.Where(u => u.Roles.Count(r => r.Name.Contains(search)) > 0).ToArray();
            var result = queryable.Where("Roles.Count(Name.Contains(@0)) > 0", search).ToArray();

            Assert.Equal(expected, result);
        }
    }
}
