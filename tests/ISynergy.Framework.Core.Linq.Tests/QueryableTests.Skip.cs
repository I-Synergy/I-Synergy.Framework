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
        /// Defines the test method Skip.
        /// </summary>
        //[TestMethod]
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
            Assert.AreEqual(testList.Skip(0).ToArray(), resultFull.Cast<User>().ToArray());
            Assert.AreEqual(testList.Skip(1).ToArray(), resultMinus1.Cast<User>().ToArray());
            Assert.AreEqual(testList.Skip(50).ToArray(), resultHalf.Cast<User>().ToArray());
            Assert.AreEqual(testList.Skip(100).ToArray(), resultNone.Cast<User>().ToArray());
        }

        /// <summary>
        /// Defines the test method SkipTestForEqualType.
        /// </summary>
        [TestMethod]
        public void SkipTestForEqualType()
        {
            // Arrange
            var testUsers = User.GenerateSampleModels(100).AsQueryable();

            // Act
            IQueryable results = testUsers.Where("Income > 10");
            var result = results.FirstOrDefault();
            Type resultType = result.GetType();

            Assert.AreEqual("ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models.User", resultType.FullName);

            var skipResult = results.Skip(1).Take(5).FirstOrDefault();
            Type skipResultType = skipResult.GetType();

            Assert.AreEqual("ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models.User", skipResultType.FullName);
        }

        /// <summary>
        /// Defines the test method SkipTestForEqualElementType.
        /// </summary>
        [TestMethod]
        public void SkipTestForEqualElementType()
        {
            // Arrange
            var testUsers = User.GenerateSampleModels(100).AsQueryable();

            // Act
            IQueryable results = testUsers.Where("Income > 10");
            var skipResult = results.Skip(1).Take(5);

            // Assert
            Assert.AreEqual(results.ElementType, skipResult.ElementType);
        }
    }
}
