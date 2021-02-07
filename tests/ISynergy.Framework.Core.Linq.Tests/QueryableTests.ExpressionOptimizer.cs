using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using NFluent;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method ExpressionOptimizerIsNull.
        /// </summary>
        [Fact]
        public void ExpressionOptimizerIsNull()
        {
            ExtensibilityPoint.QueryOptimizer = null;

            //Arrange
            var testListQry = User.GenerateSampleModels(10).AsQueryable();

            //Act
            var result = testListQry.Select<User>("it").ToArray();

            //Assert
            Check.That(result).IsNotNull();

            ExtensibilityPoint.QueryOptimizer = x => x;
        }
    }
}
