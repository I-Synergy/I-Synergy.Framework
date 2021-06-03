using System;
using System.ComponentModel;
using FluentAssertions;
using ISynergy.Framework.Core.Linq.Converters;
using ISynergy.Framework.Core.Linq.Factories;
using ISynergy.Framework.Core.Linq.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.TypeConvertors
{
    /// <summary>
    /// Class TypeConverterFactoryTests.
    /// </summary>
    [TestClass]
    public class TypeConverterFactoryTests
    {
        /// <summary>
        /// Defines the test method GetConverter_WithDefaultParsingConfig_ReturnsCorrectTypeConverter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="expected">The expected.</param>
        [DataTestMethod]
        [DataRow(typeof(DateTimeOffset), typeof(DateTimeOffsetConverter))]
        [DataRow(typeof(DateTime), typeof(DateTimeConverter))]
        [DataRow(typeof(DateTime?), typeof(NullableConverter))]
        [DataRow(typeof(int), typeof(Int32Converter))]
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
        [DataTestMethod]
        [DataRow(typeof(DateTimeOffset), typeof(DateTimeOffsetConverter))]
        [DataRow(typeof(DateTime), typeof(CustomDateTimeConverter))]
        [DataRow(typeof(DateTime?), typeof(CustomDateTimeConverter))]
        [DataRow(typeof(int), typeof(Int32Converter))]
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
