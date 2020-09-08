﻿using System;
using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void ToArray_Dynamic()
        {
            // Arrange
            var testList = User.GenerateSampleModels(51);
            IQueryable testListQry = testList.AsQueryable();

            // Act
            var realResult = testList.OrderBy(x => x.Roles.ToArray().First().Name).Select(x => x.Id);
            var testResult = testListQry.OrderBy("Roles.ToArray().First().Name").Select("Id");

            // Assert
            Assert.Equal(realResult.ToArray(), testResult.ToDynamicArray().Cast<Guid>());
        }
    }
}
