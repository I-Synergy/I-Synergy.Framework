﻿using System;
using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Skip()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var resultFull = testListQry.Skip(0);
            var resultMinus1 = testListQry.Skip(1);
            var resultHalf = testListQry.Skip(50);
            var resultNone = testListQry.Skip(100);

            //Assert
            Assert.Equal(testList.Skip(0).ToArray(), resultFull.Cast<User>().ToArray());
            Assert.Equal(testList.Skip(1).ToArray(), resultMinus1.Cast<User>().ToArray());
            Assert.Equal(testList.Skip(50).ToArray(), resultHalf.Cast<User>().ToArray());
            Assert.Equal(testList.Skip(100).ToArray(), resultNone.Cast<User>().ToArray());
        }

        [Fact]
        public void SkipTestForEqualType()
        {
            // Arrange
            var testUsers = User.GenerateSampleModels(100).AsQueryable();

            // Act
            IQueryable results = testUsers.Where("Income > 10");
            var result = results.FirstOrDefault();
            Type resultType = result.GetType();

            Assert.Equal("ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models.User", resultType.FullName);

            var skipResult = results.Skip(1).Take(5).FirstOrDefault();
            Type skipResultType = skipResult.GetType();

            Assert.Equal("ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models.User", skipResultType.FullName);
        }

        [Fact]
        public void SkipTestForEqualElementType()
        {
            // Arrange
            var testUsers = User.GenerateSampleModels(100).AsQueryable();

            // Act
            IQueryable results = testUsers.Where("Income > 10");
            var skipResult = results.Skip(1).Take(5);

            // Assert
            Assert.Equal(results.ElementType, skipResult.ElementType);
        }
    }
}
