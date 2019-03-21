using ISynergy.Framework.Tests.Base;
using System;
using Xunit;

namespace ISynergy.Extensions.Tests
{
    public class DateTimeExtensionsTests : UnitTest
    {
        [Fact]
        public void ToStartOfDayTest()
        {
            var result = new DateTime(1975, 10, 29, 14, 43, 35).ToStartOfDay();
            Assert.Equal(new DateTime(1975, 10, 29, 0, 0, 0), result);
        }

        [Fact]
        public void ToEndOfDayTest()
        {
            var result = new DateTime(1975, 10, 29, 14, 43, 35).ToEndOfDay();
            Assert.Equal(new DateTime(1975, 10, 29, 23, 59, 59, 999), result);
        }

        [Fact]
        public void ToStartOfYearTest()
        {
            var result = new DateTime().ToStartOfYear(1975);
            Assert.Equal(new DateTime(1975, 1, 1, 0, 0, 0), result);
        }

        [Fact]
        public void ToEndOfYearTest()
        {
            var result = new DateTime().ToEndOfYear(1975);
            Assert.Equal(new DateTime(1975, 12, 31, 23, 59, 59, 999), result);
        }
    }
}