using ISynergy.Framework.Core.Data.Tests.TestClasses;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Validation.Tests
{
    /// <summary>
    /// Class ArgumentTests.
    /// </summary>
    [TestClass]
    public class ArgumentTests
    {
        /// <summary>
        /// Defines the test method IsNotNullTest.
        /// </summary>
        [TestMethod]
        public void IsNotNullTest()
        {
            Product test = null;
            Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNull(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method IsNotNullOrEmptyTest.
        /// </summary>
        [TestMethod]
        public void IsNotNullOrEmptyTest()
        {
            string test = string.Empty;
            Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrEmpty(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method GuidIsNotEmptyTest.
        /// </summary>
        [TestMethod]
        public void GuidIsNotEmptyTest()
        {
            Guid test = Guid.Empty;
            Assert.ThrowsException<ArgumentException>(() => Argument.IsNotEmpty(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method GuidIsNotNullOrEmptyTest.
        /// </summary>
        [TestMethod]
        public void GuidIsNotNullOrEmptyTest()
        {
            Guid test = default;
            Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrEmpty(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method IsNotNullOrWhitespaceTest.
        /// </summary>
        [TestMethod]
        public void IsNotNullOrWhitespaceTest()
        {
            string test = " ";
            Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrWhitespace(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method IsNotNullOrEmptyArrayTest.
        /// </summary>
        [TestMethod]
        public void IsNotNullOrEmptyArrayTest()
        {
            Array test = Array.Empty<object>();
            Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrEmptyArray(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method IsNotNullOrEmptyListTTest.
        /// </summary>
        [TestMethod]
        public void IsNotNullOrEmptyListTTest()
        {
            var test = new List<object>();
            Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrEmptyList(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method IsNotEnumTTest.
        /// </summary>
        [TestMethod]
        public void IsNotEnumTTest()
        {
            var test = new object();
            Assert.ThrowsException<ArgumentException>(() => Argument.IsNotEnum(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method HasNoNullsTTest.
        /// </summary>
        [TestMethod]
        public void HasNoNullsTTest()
        {
            var test = new List<Product>();
            test.Add(new Product());
            test.Add(null);
            
            Assert.ThrowsException<ArgumentNullException>(() => Argument.HasNoNulls(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method IsNotOutOfRangeTTest.
        /// </summary>
        [TestMethod]
        public void IsNotOutOfRangeTTest()
        {
            var test = 1975;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Argument.IsNotOutOfRange(nameof(test), test, 2000, 2021));
        }

        /// <summary>
        /// Defines the test method IsMinimalTTest.
        /// </summary>
        [TestMethod]
        public void IsMinimalTTest()
        {
            var test = 1975;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Argument.IsMinimal(nameof(test), test, 2000));
        }

        /// <summary>
        /// Defines the test method IsMaximumTTest.
        /// </summary>
        [TestMethod]
        public void IsMaximumTTest()
        {
            var test = 1975;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Argument.IsMaximum(nameof(test), test, 1970));
        }
    }
}
