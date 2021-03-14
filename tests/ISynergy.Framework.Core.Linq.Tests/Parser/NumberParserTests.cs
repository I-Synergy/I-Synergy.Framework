using System.Globalization;
using ISynergy.Framework.Core.Linq.Parsers;
using NFluent;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Parser
{
    /// <summary>
    /// Class NumberParserTests.
    /// </summary>
    public class NumberParserTests
    {
        /// <summary>
        /// The parsing configuration
        /// </summary>
        private readonly ParsingConfig _parsingConfig = new ParsingConfig();

        /// <summary>
        /// The sut
        /// </summary>
        private readonly NumberParser _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberParserTests"/> class.
        /// </summary>
        public NumberParserTests()
        {
            _sut = new NumberParser(_parsingConfig);
        }

        /// <summary>
        /// Defines the test method NumberParser_ParseNumber_Decimal_With_DefaultCulture.
        /// </summary>
        [Fact]
        public void NumberParser_ParseNumber_Decimal_With_DefaultCulture()
        {
            // Act
            var result = _sut.ParseNumber("3.21", typeof(decimal));

            // Assert
            Check.That(result).Equals(3.21m);
        }

        /// <summary>
        /// Defines the test method NumberParser_ParseNumber_Decimal_With_GermanCulture.
        /// </summary>
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
