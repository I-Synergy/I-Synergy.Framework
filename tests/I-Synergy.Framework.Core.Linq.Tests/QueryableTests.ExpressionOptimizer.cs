using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using NFluent;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public partial class QueryableTests
    {
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
