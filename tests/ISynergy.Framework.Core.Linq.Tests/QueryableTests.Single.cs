using System;
using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Single()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.Take(1).Single();

            //Assert
            Assert.Equal(testList[0].Id, result.Id);
        }

        [Fact]
        public void Single_Predicate()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var testListQry = testList.AsQueryable();

            //Act
            var expected = testListQry.Single(u => u.UserName == "User4");
            var result = testListQry.Single("UserName == \"User4\"");

            //Assert
            Assert.Equal(expected as object, result);
        }

        [Fact]
        public void SingleOrDefault()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var singleResult = testListQry.Take(1).SingleOrDefault();
            var defaultResult = ((IQueryable)Enumerable.Empty<User>().AsQueryable()).SingleOrDefault();

            //Assert
            Assert.Equal(testList[0].Id, singleResult.Id);
            Assert.Null(defaultResult);
        }

        [Fact]
        public void SingleOrDefault_Predicate()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var testListQry = testList.AsQueryable();

            //Act
            var expected = testListQry.SingleOrDefault(u => u.UserName == "User4");
            var result = testListQry.SingleOrDefault("UserName == \"User4\"");

            //Assert
            Assert.Equal(expected as object, result);
        }

        [Fact]
        public void Single_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(1);
            while (testList[0].Roles.Count > 1) testList[0].Roles.RemoveAt(0);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            dynamic realResult = testList.OrderBy(x => x.Roles.Single().Name).Select(x => x.Id).ToArray();
            var testResult = testListQry.OrderBy("Roles.Single().Name").Select("Id");

            //Assert
            Assert.Equal(realResult, testResult.Cast<Guid>().ToArray());
        }
    }
}
