using System;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using ISynergy.Framework.Core.Extensions;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class ObjectExtensionTests.
    /// </summary>
    public class ObjectExtensionTests
    {
        /// <summary>
        /// Defines the test method NonNullableTypeTest.
        /// </summary>
        /// <param name="type">The type.</param>
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(bool))]
        public void NonNullableTypeTest(Type type)
        {
            Assert.False(type.IsNullableType());
        }

        /// <summary>
        /// Defines the test method NullableTypeTest.
        /// </summary>
        /// <param name="type">The type.</param>
        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(object))]
        [InlineData(typeof(Product))]
        public void NullableTypeTest(Type type)
        {
            Assert.True(type.IsNullableType());
        }
    }
}
