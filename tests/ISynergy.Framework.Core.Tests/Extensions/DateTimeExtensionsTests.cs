using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class DateTimeExtensionsTests.
    /// </summary>
    [TestClass]
    public class DateTimeExtensionsTests
    {
        /// <summary>
        /// Defines the test method ToStartOfDayTest.
        /// </summary>
        [TestMethod]
        public void ToStartOfDayTest()
        {
            var result = new DateTime(1975, 10, 29, 14, 43, 35).ToStartOfDay();
            Assert.AreEqual(new DateTime(1975, 10, 29, 0, 0, 0), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfDayTest.
        /// </summary>
        [TestMethod]
        public void ToEndOfDayTest()
        {
            var result = new DateTime(1975, 10, 29, 14, 43, 35).ToEndOfDay();
            Assert.AreEqual(new DateTime(1975, 10, 29, 0, 0, 0, 0).AddDays(1).AddTicks(-1), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfMontTest.
        /// </summary>
        [TestMethod]
        public void ToStartOfMontTest()
        {
            var result = new DateTime(1975, 10, 29, 14, 43, 35).ToStartOfMonth();
            Assert.AreEqual(new DateTime(1975, 10, 1, 0, 0, 0), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfMonthTest.
        /// </summary>
        [TestMethod]
        public void ToEndOfMonthTest()
        {
            var result = new DateTime(1975, 10, 29, 14, 43, 35).ToEndOfMonth();
            Assert.AreEqual(new DateTime(1975, 10, 31, 0, 0, 0, 0).AddDays(1).AddTicks(-1), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfYearTest.
        /// </summary>
        [TestMethod]
        public void ToStartOfYearTest()
        {
            var result = new DateTime(1975, 1, 1).Year.ToStartOfYear();
            Assert.AreEqual(new DateTime(1975, 1, 1, 0, 0, 0), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfYearTest.
        /// </summary>
        [TestMethod]
        public void ToEndOfYearTest()
        {
            var result = new DateTime(1975, 1, 1).Year.ToEndOfYear();
            Assert.AreEqual(new DateTime(1975, 1, 1).AddYears(1).AddTicks(-1), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfWeekOnMondayTest.
        /// </summary>
        [TestMethod]
        public void ToStartOfWeekOnMondayTest()
        {
            var result = new DateTime(1975, 10, 1, 14, 43, 35).ToStartOfWeek(DayOfWeek.Monday);
            Assert.AreEqual(new DateTime(1975, 9, 29).ToStartOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfWeekOnMondayTest.
        /// </summary>
        [TestMethod]
        public void ToEndOfWeekOnMondayTest()
        {
            var result = new DateTime(1975, 10, 1, 14, 43, 35).ToEndOfWeek(DayOfWeek.Monday);
            Assert.AreEqual(new DateTime(1975, 10, 5).ToEndOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfWeekOnSundayTest.
        /// </summary>
        [TestMethod]
        public void ToStartOfWeekOnSundayTest()
        {
            var result = new DateTime(1975, 10, 1, 14, 43, 35).ToStartOfWeek(DayOfWeek.Sunday);
            Assert.AreEqual(new DateTime(1975, 9, 28).ToStartOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfWeekOnSundayTest.
        /// </summary>
        [TestMethod]
        public void ToEndOfWeekOnSundayTest()
        {
            var result = new DateTime(1975, 10, 1, 14, 43, 35).ToEndOfWeek(DayOfWeek.Sunday);
            Assert.AreEqual(new DateTime(1975, 10, 4).ToEndOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfWorkWeekTest.
        /// </summary>
        [TestMethod]
        public void ToStartOfWorkWeekTest()
        {
            var result = new DateTime(1975, 10, 4, 14, 43, 35).ToStartOfWorkWeek();
            Assert.AreEqual(new DateTime(1975, 9, 29).ToStartOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfWorkWeekTest.
        /// </summary>
        [TestMethod]
        public void ToEndOfWorkWeekTest()
        {
            var result = new DateTime(1975, 10, 1, 14, 43, 35).ToEndOfWorkWeek();
            Assert.AreEqual(new DateTime(1975, 10, 3).ToEndOfDay(), result);
        }
    }
}
