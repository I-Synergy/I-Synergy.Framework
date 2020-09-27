using System;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using ISynergy.Framework.Core.Extensions;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    public class ObjectExtensionTests
    {
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(bool))]
        public void NonNullableTypeTest(Type type)
        {
            Assert.False(type.IsNullableType());
        }

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
