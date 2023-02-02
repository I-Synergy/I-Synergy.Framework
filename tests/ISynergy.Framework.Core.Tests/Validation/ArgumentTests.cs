using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using ISynergy.Framework.Core.Locators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

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
            Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNull(test));
        }

        /// <summary>
        /// Defines the test method IsNotNullOrEmptyTest.
        /// </summary>
        [TestMethod]
        public void IsNotNullOrEmptyTest()
        {
            string test = string.Empty;
            Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrEmpty(test));
        }

        /// <summary>
        /// Defines the test method GuidIsNotEmptyTest.
        /// </summary>
        [TestMethod]
        public void GuidIsNotEmptyTest()
        {
            Guid test = Guid.Empty;
            Assert.ThrowsException<ArgumentException>(() => Argument.IsNotEmpty(test));
        }

        /// <summary>
        /// Defines the test method GuidIsNotNullOrEmptyTest.
        /// </summary>
        [TestMethod]
        public void GuidIsNotNullOrEmptyTest()
        {
            Guid test = default;
            Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrEmpty(test));
        }

        /// <summary>
        /// Defines the test method IsNotNullOrWhitespaceTest.
        /// </summary>
        [TestMethod]
        public void IsNotNullOrWhitespaceTest()
        {
            string test = " ";
            Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrWhitespace(test));
        }

        /// <summary>
        /// Defines the test method IsNotNullOrEmptyArrayTest.
        /// </summary>
        [TestMethod]
        public void IsNotNullOrEmptyArrayTest()
        {
            Array test = Array.Empty<object>();
            Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrEmptyArray(test));
        }

        /// <summary>
        /// Defines the test method IsNotNullOrEmptyListTTest.
        /// </summary>
        [TestMethod]
        public void IsNotNullOrEmptyListTTest()
        {
            var test = new List<object>();
            Assert.ThrowsException<ArgumentNullException>(() => Argument.IsNotNullOrEmptyList(test));
        }

        /// <summary>
        /// Defines the test method IsNotEnumTTest.
        /// </summary>
        [TestMethod]
        public void IsNotEnumTTest()
        {
            var test = new object();
            Assert.ThrowsException<ArgumentException>(() => Argument.IsNotEnum(test));
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
            
            Assert.ThrowsException<ArgumentNullException>(() => Argument.HasNoNulls(test));
        }

        /// <summary>
        /// Defines the test method IsNotOutOfRangeTTest.
        /// </summary>
        [TestMethod]
        public void IsNotOutOfRangeTTest()
        {
            var test = 1975;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Argument.IsNotOutOfRange(test, 2000, 2021));
        }

        /// <summary>
        /// Defines the test method IsMinimalTTest.
        /// </summary>
        [TestMethod]
        public void IsMinimalTTest()
        {
            var test = 1975;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Argument.IsMinimal(test, 2000));
        }

        /// <summary>
        /// Defines the test method IsMaximumTTest.
        /// </summary>
        [TestMethod]
        public void IsMaximumTTest()
        {
            var test = 1975;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Argument.IsMaximum(test, 1970));
        }
    }
}
