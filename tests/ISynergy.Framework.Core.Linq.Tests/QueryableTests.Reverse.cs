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
        /// Defines the test method Reverse.
        /// </summary>
        //[TestMethod]
        public void Reverse()
        {
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.Reverse();

            //Assert
            Assert.AreEqual(testList.Reverse().ToArray(), result.Cast<User>().ToArray());
        }
    }
}
