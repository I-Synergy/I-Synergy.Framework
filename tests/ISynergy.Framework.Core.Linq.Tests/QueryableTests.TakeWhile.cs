using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method TakeWhile_Predicate.
        /// </summary>
        [Fact]
        public void TakeWhile_Predicate()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var expected = testList.TakeWhile(u => u.Income > 1000);
            var result = testListQry.TakeWhile("Income > 1000");

            //Assert
            Assert.Equal(expected.ToArray(), result.Cast<User>().ToArray());
        }

        /// <summary>
        /// Defines the test method TakeWhile_Predicate_Args.
        /// </summary>
        [Fact]
        public void TakeWhile_Predicate_Args()
        {
            const int income = 1000;

            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var expected = testList.TakeWhile(u => u.Income > income);
            var result = testListQry.TakeWhile("Income > @0", income);

            //Assert
            Assert.Equal(expected.ToArray(), result.Cast<User>().ToArray());
        }
    }
}
