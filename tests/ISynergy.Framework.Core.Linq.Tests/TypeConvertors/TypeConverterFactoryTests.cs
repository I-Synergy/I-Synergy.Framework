using System;
using System.ComponentModel;
using FluentAssertions;
using ISynergy.Framework.Core.Linq.Converters;
using ISynergy.Framework.Core.Linq.Factories;
using ISynergy.Framework.Core.Linq.Parsers;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.TypeConvertors
{
    /// <summary>
    /// Class TypeConverterFactoryTests.
    /// </summary>
    public class TypeConverterFactoryTests
    {
        /// <summary>
        /// Defines the test method GetConverter_WithDefaultParsingConfig_ReturnsCorrectTypeConverter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="expected">The expected.</param>
        [Theory]
        [InlineData(typeof(DateTimeOffset), typeof(DateTimeOffsetConverter))]
        [InlineData(typeof(DateTime), typeof(DateTimeConverter))]
        [InlineData(typeof(DateTime?), typeof(NullableConverter))]
        [InlineData(typeof(int), typeof(Int32Converter))]
        public void GetConverter_WithDefaultParsingConfig_ReturnsCorrectTypeConverter(Type type, Type expected)
        {
            // Arrange
            var factory = new TypeConverterFactory(ParsingConfig.Default);

            // Act
            var typeConverter = factory.GetConverter(type);

            // Assert
            typeConverter.Should().BeOfType(expected);
        }

        /// <summary>
        /// Defines the test method GetConverter_WithDateTimeIsParsedAsUTCIsTrue_ReturnsCorrectTypeConverter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="expected">The expected.</param>
        [Theory]
        [InlineData(typeof(DateTimeOffset), typeof(DateTimeOffsetConverter))]
        [InlineData(typeof(DateTime), typeof(CustomDateTimeConverter))]
        [InlineData(typeof(DateTime?), typeof(CustomDateTimeConverter))]
        [InlineData(typeof(int), typeof(Int32Converter))]
        public void GetConverter_WithDateTimeIsParsedAsUTCIsTrue_ReturnsCorrectTypeConverter(Type type, Type expected)
        {
            // Arrange
            var parsingConfig = new ParsingConfig
            {
                DateTimeIsParsedAsUTC = true
            };
            var factory = new TypeConverterFactory(parsingConfig);

            // Act
            var typeConverter = factory.GetConverter(type);

            // Assert
            typeConverter.Should().BeOfType(expected);
        }
    }
}
