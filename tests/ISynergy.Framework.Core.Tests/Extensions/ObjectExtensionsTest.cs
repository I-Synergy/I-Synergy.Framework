using ISynergy.Framework.Core.Data.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class ObjectExtensionTests.
    /// </summary>
    [TestClass]
    public class ObjectExtensionTests
    {
        /// <summary>
        /// Defines the test method NonNullableTypeTest.
        /// </summary>
        /// <param name="type">The type.</param>
        [DataTestMethod]
        [DataRow(typeof(int))]
        [DataRow(typeof(bool))]
        public void NonNullableTypeTest(Type type)
        {
            Assert.IsFalse(type.IsNullableType());
        }

        /// <summary>
        /// Defines the test method NullableTypeTest.
        /// </summary>
        /// <param name="type">The type.</param>
        [DataTestMethod]
        [DataRow(typeof(string))]
        [DataRow(typeof(object))]
        [DataRow(typeof(Product))]
        public void NullableTypeTest(Type type)
        {
            Assert.IsTrue(type.IsNullableType());
        }
    }
}
