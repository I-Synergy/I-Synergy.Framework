using Xunit;

namespace ISynergy.Framework.Core.Utilities
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
            Assert.False(FileNameUtility.IsValidFileName(fileName));
        }

        [Theory]
        [InlineData(".test")]
        [InlineData("test")]
        public void TestIsValidFileName(string fileName)
        {
            Assert.True(FileNameUtility.IsValidFileName(fileName));
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
            Assert.True(FileNameUtility.IsValidFileName(FileNameUtility.MakeValidFileName(fileName)));
        }
    }
}
