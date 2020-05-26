﻿using System.Globalization;
using ISynergy.Framework.Core.Linq.Parsers;
using NFluent;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Parser
{
    public class NumberParserTests
    {
        private readonly ParsingConfig _parsingConfig = new ParsingConfig();

        private readonly NumberParser _sut;

        public NumberParserTests()
        {
            _sut = new NumberParser(_parsingConfig);
        }

        [Fact]
        public void NumberParser_ParseNumber_Decimal_With_DefaultCulture()
        {
            // Act
            var result = _sut.ParseNumber("3.21", typeof(decimal));

            // Assert
            Check.That(result).Equals(3.21m);
        }

        [Fact]
        public void NumberParser_ParseNumber_Decimal_With_GermanCulture()
        {
            // Assign
            _parsingConfig.NumberParseCulture = CultureInfo.CreateSpecificCulture("de-DE");

            // Act
            var result = _sut.ParseNumber("3,21", typeof(decimal));

            // Assert
            Check.That(result).Equals(3.21m);
        }
    }
}
