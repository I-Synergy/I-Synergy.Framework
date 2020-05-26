﻿using System.Linq;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Parsers;
using NFluent;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public class ParsingConfigTests
    {
        class TestQueryableAnalyzer : IQueryableAnalyzer
        {
            public bool SupportsLinqToObjects(IQueryable query, IQueryProvider provider)
            {
                return true;
            }
        }

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
