using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ISynergy.Framework.UI.Converters.Tests
{
    [TestClass()]
    public class DateTimeOffsetToTimeSpanConverterTests
    {
        /// <summary>
        /// Create unit test for <see cref="DateTimeOffsetToTimeSpanConverter.Convert(object, Type, object, CultureInfo)"/> method.
        /// </summary>
        [TestMethod()]
        public void ConvertTest()
        {
            // Arrange
            var dateTimeOffset = new DateTimeOffset(1975, 10, 29, 15, 0, 0, TimeSpan.Zero);
            var converter = new DateTimeOffsetToTimeSpanConverter();
            // Act
            var result = converter.Convert(dateTimeOffset, typeof(TimeSpan), null, null);
            // Assert
            Assert.AreEqual(dateTimeOffset.TimeOfDay, result);
        }

        /// <summary>
        /// Create unit test for <see cref="DateTimeOffsetToTimeSpanConverter.ConvertBack(object, Type, object, CultureInfo)"/> method.
        /// </summary>
        [TestMethod()]
        public void ConvertBackTest()
        {
            // Arrange
            var converter = new DateTimeOffsetToTimeSpanConverter();
            var dateTimeOffset = new DateTimeOffset(1975, 10, 29, 0, 0, 0, TimeSpan.Zero);
            var timeSpan = new TimeSpan(15, 2, 3);
            // Act
            var result = converter.ConvertBack(dateTimeOffset, typeof(TimeSpan), timeSpan, null);
            // Assert
            Assert.AreEqual(new DateTimeOffset(1975, 10, 29, 15, 2, 3, TimeSpan.Zero), result);
        }
    }
}