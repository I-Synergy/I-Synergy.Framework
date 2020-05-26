using System;
using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Last()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var realResult = testList.Last();
            var result = testListQry.Last();

            //Assert
            Assert.Equal(realResult.Id, result.Id);
        }

        [Fact]
        public void Last_Predicate()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            //Act
            var expected = queryable.Last(u => u.Income > 1000);
            var result = queryable.Last("Income > 1000");

            //Assert
            Assert.Equal(expected as object, result);
        }

        [Fact]
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
            Assert.Equal(realResult.Id, singleResult.Id);
            Assert.Null(defaultResult);
        }

        [Fact]
        public void LastOrDefault_Predicate()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            //Act
            var expected = queryable.LastOrDefault(u => u.Income > 1000);
            var result = queryable.LastOrDefault("Income > 1000");

            //Assert
            Assert.Equal(expected as object, result);
        }

        [Fact]
        public void Last_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var realResult = testList.OrderBy(x => x.Roles.Last().Name).Select(x => x.Id).ToArray();
            var testResult = testListQry.OrderBy("Roles.Last().Name").Select("Id");

            //Assert
            Assert.Equal(realResult, testResult.Cast<Guid>().ToArray());
        }
    }
}
