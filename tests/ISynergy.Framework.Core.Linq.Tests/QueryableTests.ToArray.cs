using System;
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
        /// Defines the test method ToArray_Dynamic.
        /// </summary>
        //[TestMethod]
        public void ToArray_Dynamic()
        {
            // Arrange
            var testList = User.GenerateSampleModels(51);
            IQueryable testListQry = testList.AsQueryable();

            // Act
            var realResult = testList.OrderBy(x => x.Roles.ToArray().First().Name).Select(x => x.Id);
            var testResult = testListQry.OrderBy("Roles.ToArray().First().Name").Select("Id");

            // Assert
            Assert.AreEqual(realResult.ToArray(), testResult.ToDynamicArray().Cast<Guid>());
        }
    }
}
