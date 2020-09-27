using System;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using Xunit;

namespace ISynergy.Framework.Core.Utilities.Tests
{
    public class TypeExtensionsTests
    {
        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(object))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(Product))]
        public void BasicTypeActivatorTest(Type type)
        {
            var result = TypeActivator.CreateInstance(type);
            Assert.NotNull(result);
        }

    }
}
