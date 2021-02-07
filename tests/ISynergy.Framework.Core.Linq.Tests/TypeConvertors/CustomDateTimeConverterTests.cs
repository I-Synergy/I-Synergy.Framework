using System;
using FluentAssertions;
using ISynergy.Framework.Core.Linq.Converters;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.TypeConvertors
{
    /// <summary>
    /// Class CustomDateTimeConverterTests.
    /// </summary>
    public class CustomDateTimeConverterTests
    {
        /// <summary>
        /// The sut
        /// </summary>
        private readonly CustomDateTimeConverter _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDateTimeConverterTests"/> class.
        /// </summary>
        public CustomDateTimeConverterTests()
        {
            _sut = new CustomDateTimeConverter();
        }

        /// <summary>
        /// Defines the test method ConvertFromInvariantString_ReturnsCorrectDateTime.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="hours">The hours.</param>
        [Theory]
        [InlineData("Fri, 10 May 2019 11:03:17 GMT", 11)]
        [InlineData("Fri, 10 May 2019 11:03:17 -07:00", 18)]
        public void ConvertFromInvariantString_ReturnsCorrectDateTime(string value, int hours)
        {
            // Act
            DateTime? result = _sut.ConvertFromInvariantString(value) as DateTime?;

            // Assert
            result.Should().Be(new DateTime(2019, 5, 10, hours, 3, 17));
        }
    }
}
