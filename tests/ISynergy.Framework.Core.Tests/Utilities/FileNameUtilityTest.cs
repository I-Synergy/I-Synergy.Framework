using ISynergy.Framework.Core.Extensions;
using Xunit;

namespace ISynergy.Framework.Core.Utilities.Tests
{
    /// <summary>
    /// Class FileNameUtilityTests.
    /// </summary>
    public class FileNameUtilityTests
    {
        /// <summary>
        /// Defines the test method TestIsNotValidFileName.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
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

        /// <summary>
        /// Defines the test method TestIsValidFileName.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        [Theory]
        [InlineData(".test")]
        [InlineData("test")]
        public void TestIsValidFileName(string fileName)
        {
            Assert.True(FileNameExtensions.IsValidFileName(fileName));
        }

        /// <summary>
        /// Defines the test method CreateValidFileName.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
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
