using ISynergy.Framework.Core.Extensions;
using Xunit;

namespace ISynergy.Framework.Core.Utilities.Tests
{
    public class FileNameUtilityTests
    {
        [Theory]
        [InlineData("test?")]
        [InlineData("test.")]
        [InlineData("/test")]
        [InlineData("\\test")]
        [InlineData("test/")]
        [InlineData("test\\")]
        [InlineData("test\\test")]
        public void TestIsNotValidFileName(string fileName)
        {
            Assert.False(FileNameExtensions.IsValidFileName(fileName));
        }

        [Theory]
        [InlineData(".test")]
        [InlineData("test")]
        public void TestIsValidFileName(string fileName)
        {
            Assert.True(FileNameExtensions.IsValidFileName(fileName));
        }

        [Theory]
        [InlineData("test?")]
        [InlineData(".test")]
        [InlineData("test.")]
        [InlineData("/test")]
        [InlineData("\\test")]
        [InlineData("test/")]
        [InlineData("test\\")]
        [InlineData("test\\test")]
        [InlineData("test")]
        public void CreateValidFileName(string fileName)
        {
            Assert.True(FileNameExtensions.IsValidFileName(FileNameExtensions.MakeValidFileName(fileName)));
        }
    }
}
