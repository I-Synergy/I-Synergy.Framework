using System.Linq;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Parsers;
using NFluent;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class ParsingConfigTests.
    /// </summary>
    public class ParsingConfigTests
    {
        /// <summary>
        /// Class TestQueryableAnalyzer.
        /// Implements the <see cref="ISynergy.Framework.Core.Linq.Abstractions.IQueryableAnalyzer" />
        /// </summary>
        /// <seealso cref="ISynergy.Framework.Core.Linq.Abstractions.IQueryableAnalyzer" />
        class TestQueryableAnalyzer : IQueryableAnalyzer
        {
            /// <summary>
            /// Determines whether the specified query (and provider) supports LinqToObjects.
            /// </summary>
            /// <param name="query">The query to check.</param>
            /// <param name="provider">The provider to check (can be null).</param>
            /// <returns>true/false</returns>
            public bool SupportsLinqToObjects(IQueryable query, IQueryProvider provider)
            {
                return true;
            }
        }

        /// <summary>
        /// Defines the test method ParsingConfig_QueryableAnalyzer_Set_Null.
        /// </summary>
        [Fact]
        public void ParsingConfig_QueryableAnalyzer_Set_Null()
        {
            // Assign
            var config = ParsingConfig.Default;

            // Act
            config.QueryableAnalyzer = null;

            // Assert
            Check.That(config.QueryableAnalyzer).IsNotNull();
        }

        /// <summary>
        /// Defines the test method ParsingConfig_QueryableAnalyzer_Set_Custom.
        /// </summary>
        [Fact]
        public void ParsingConfig_QueryableAnalyzer_Set_Custom()
        {
            // Assign
            var config = ParsingConfig.Default;
            var analyzer = new TestQueryableAnalyzer();

            // Act
            config.QueryableAnalyzer = analyzer;

            // Assert
            Check.That(config.QueryableAnalyzer).IsEqualTo(analyzer);
        }
    }
}
