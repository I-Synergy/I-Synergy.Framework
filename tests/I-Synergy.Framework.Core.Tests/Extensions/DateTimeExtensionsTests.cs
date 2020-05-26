using System;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    public class DateTimeExtensionsTests
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
            Assert.Equal(new DateTime(1975, 10, 29, 0, 0, 0, 0).AddDays(1).AddTicks(-1), result);
        }

        [Fact]
        public void ToStartOfMontTest()
        {
            var result = new DateTime(1975, 10, 29, 14, 43, 35).ToStartOfMonth();
            Assert.Equal(new DateTime(1975, 10, 1, 0, 0, 0), result);
        }

        [Fact]
        public void ToEndOfMonthTest()
        {
            var result = new DateTime(1975, 10, 29, 14, 43, 35).ToEndOfMonth();
            Assert.Equal(new DateTime(1975, 10, 31, 0, 0, 0, 0).AddDays(1).AddTicks(-1), result);
        }

        [Fact]
        public void ToStartOfYearTest()
        {
            var result = new DateTime(1975, 1, 1).Year.ToStartOfYear();
            Assert.Equal(new DateTime(1975, 1, 1, 0, 0, 0), result);
        }

        [Fact]
        public void ToEndOfYearTest()
        {
            var result = new DateTime(1975, 1, 1).Year.ToEndOfYear();
            Assert.Equal(new DateTime(1975, 1, 1).AddYears(1).AddTicks(-1), result);
        }

        [Fact]
        public void ToStartOfWeekOnMondayTest()
        {
            var result = new DateTime(1975, 10, 1, 14, 43, 35).ToStartOfWeek(DayOfWeek.Monday);
            Assert.Equal(new DateTime(1975, 9, 29).ToStartOfDay(), result);
        }

        [Fact]
        public void ToEndOfWeekOnMondayTest()
        {
            var result = new DateTime(1975, 10, 1, 14, 43, 35).ToEndOfWeek(DayOfWeek.Monday);
            Assert.Equal(new DateTime(1975, 10, 5).ToEndOfDay(), result);
        }

        [Fact]
        public void ToStartOfWeekOnSundayTest()
        {
            var result = new DateTime(1975, 10, 1, 14, 43, 35).ToStartOfWeek(DayOfWeek.Sunday);
            Assert.Equal(new DateTime(1975, 9, 28).ToStartOfDay(), result);
        }

        [Fact]
        public void ToEndOfWeekOnSundayTest()
        {
            var result = new DateTime(1975, 10, 1, 14, 43, 35).ToEndOfWeek(DayOfWeek.Sunday);
            Assert.Equal(new DateTime(1975, 10, 4).ToEndOfDay(), result);
        }

        [Fact]
        public void ToStartOfWorkWeekTest()
        {
            var result = new DateTime(1975, 10, 4, 14, 43, 35).ToStartOfWorkWeek();
            Assert.Equal(new DateTime(1975, 9, 29).ToStartOfDay(), result);
        }

        [Fact]
        public void ToEndOfWorkWeekTest()
        {
            var result = new DateTime(1975, 10, 1, 14, 43, 35).ToEndOfWorkWeek();
            Assert.Equal(new DateTime(1975, 10, 3).ToEndOfDay(), result);
        }
    }
}
