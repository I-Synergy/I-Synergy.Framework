using System;
using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void First()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.First();

            //Assert
            Assert.Equal(testList[0].Id, result.Id);
        }

        [Fact]
        public void First_Predicate()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            //Act
            var expected = queryable.First(u => u.Income > 1000);
            var result = queryable.First("Income > 1000");

            //Assert
            Assert.Equal(expected as object, result);
        }

        [Fact]
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
            Assert.Equal(expected as object, result);
        }

        [Fact]
        public void First_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var realResult = testList.OrderBy(x => x.Roles.First().Name).Select(x => x.Id).ToArray();
            var testResult = testListQry.OrderBy("Roles.First().Name").Select("Id");

            //Assert
            Assert.Equal(realResult, testResult.Cast<Guid>().ToArray());
        }
    }
}
