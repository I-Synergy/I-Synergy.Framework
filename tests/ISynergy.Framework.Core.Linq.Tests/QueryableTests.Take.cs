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
        /// Defines the test method Take.
        /// </summary>
        //[TestMethod]
        public void Take()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var resultFull = testListQry.Take(100);
            var resultMinus1 = testListQry.Take(99);
            var resultHalf = testListQry.Take(50);
            var resultOne = testListQry.Take(1);

            //Assert
            Assert.AreEqual(testList.Take(100).ToArray(), resultFull.Cast<User>().ToArray());
            Assert.AreEqual(testList.Take(99).ToArray(), resultMinus1.Cast<User>().ToArray());
            Assert.AreEqual(testList.Take(50).ToArray(), resultHalf.Cast<User>().ToArray());
            Assert.AreEqual(testList.Take(1).ToArray(), resultOne.Cast<User>().ToArray());
        }
    }
}
