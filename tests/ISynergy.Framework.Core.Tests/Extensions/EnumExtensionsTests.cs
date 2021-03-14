using ISynergy.Framework.Core.Tests.Data.TestClasses;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class EnumExtensionsTests.
    /// </summary>
    public class EnumExtensionsTests
    {
        /// <summary>
        /// Defines the test method GetDescriptionTest.
        /// </summary>
        [Fact]
        public void GetDescriptionTest()
        {
            Assert.Equal("Description1", ProductTypes.Type1.GetDescription());
            Assert.Equal("Description2", ProductTypes.Type2.GetDescription());
            Assert.Equal("Description3", ProductTypes.Type3.GetDescription());
        }
    }
}
