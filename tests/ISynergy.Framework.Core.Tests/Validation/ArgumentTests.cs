using ISynergy.Framework.Core.Data.Tests.TestClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Framework.Core.Validation.Tests
{
    /// <summary>
    /// Class ArgumentTests.
    /// </summary>
    public class ArgumentTests
    {
        /// <summary>
        /// Defines the test method IsNotNullTest.
        /// </summary>
        [Fact]
        public void IsNotNullTest()
        {
            Product test = null;
            Assert.Throws<ArgumentNullException>(() => Argument.IsNotNull(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method IsNotNullOrEmptyTest.
        /// </summary>
        [Fact]
        public void IsNotNullOrEmptyTest()
        {
            string test = string.Empty;
            Assert.Throws<ArgumentNullException>(() => Argument.IsNotNullOrEmpty(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method GuidIsNotEmptyTest.
        /// </summary>
        [Fact]
        public void GuidIsNotEmptyTest()
        {
            Guid test = Guid.Empty;
            Assert.Throws<ArgumentException>(() => Argument.IsNotEmpty(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method GuidIsNotNullOrEmptyTest.
        /// </summary>
        [Fact]
        public void GuidIsNotNullOrEmptyTest()
        {
            Guid test = default;
            Assert.Throws<ArgumentException>(() => Argument.IsNotNullOrEmpty(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method IsNotNullOrWhitespaceTest.
        /// </summary>
        [Fact]
        public void IsNotNullOrWhitespaceTest()
        {
            string test = " ";
            Assert.Throws<ArgumentException>(() => Argument.IsNotNullOrWhitespace(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method IsNotNullOrEmptyArrayTest.
        /// </summary>
        [Fact]
        public void IsNotNullOrEmptyArrayTest()
        {
            Array test = Array.Empty<object>();
            Assert.Throws<ArgumentException>(() => Argument.IsNotNullOrEmptyArray(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method IsNotNullOrEmptyListTTest.
        /// </summary>
        [Fact]
        public void IsNotNullOrEmptyListTTest()
        {
            var test = new List<object>();
            Assert.Throws<ArgumentException>(() => Argument.IsNotNullOrEmptyList(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method IsNotEnumTTest.
        /// </summary>
        [Fact]
        public void IsNotEnumTTest()
        {
            var test = new object();
            Assert.Throws<ArgumentException>(() => Argument.IsNotEnum(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method HasNoNullsTTest.
        /// </summary>
        [Fact]
        public void HasNoNullsTTest()
        {
            var test = new List<Product>();
            test.Add(new Product());
            test.Add(null);
            
            Assert.Throws<ArgumentNullException>(() => Argument.HasNoNulls(nameof(test), test));
        }

        /// <summary>
        /// Defines the test method IsNotOutOfRangeTTest.
        /// </summary>
        [Fact]
        public void IsNotOutOfRangeTTest()
        {
            var test = 1975;
            Assert.Throws<ArgumentOutOfRangeException>(() => Argument.IsNotOutOfRange(nameof(test), test, 2000, 2021));
        }

        /// <summary>
        /// Defines the test method IsMinimalTTest.
        /// </summary>
        [Fact]
        public void IsMinimalTTest()
        {
            var test = 1975;
            Assert.Throws<ArgumentOutOfRangeException>(() => Argument.IsMinimal(nameof(test), test, 2000));
        }

        /// <summary>
        /// Defines the test method IsMaximumTTest.
        /// </summary>
        [Fact]
        public void IsMaximumTTest()
        {
            var test = 1975;
            Assert.Throws<ArgumentOutOfRangeException>(() => Argument.IsMaximum(nameof(test), test, 1970));
        }
    }
}
