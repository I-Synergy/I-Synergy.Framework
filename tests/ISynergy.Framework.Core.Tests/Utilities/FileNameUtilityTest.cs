using ISynergy.Framework.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Tests.Utilities
{
    /// <summary>
    /// Class FileNameUtilityTests.
    /// </summary>
    [TestClass]
    public class FileNameUtilityTests
    {
        /// <summary>
        /// Defines the test method TestIsNotValidFileName.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        [DataTestMethod]
        [DataRow("test?")]
        [DataRow("test.")]
        [DataRow("/test")]
        [DataRow("\\test")]
        [DataRow("test/")]
        [DataRow("test\\")]
        [DataRow("test\\test")]
        public void TestIsNotValidFileName(string fileName)
        {
            Assert.IsFalse(fileName.IsValidFileName());
        }

        /// <summary>
        /// Defines the test method TestIsValidFileName.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        [DataTestMethod]
        [DataRow(".test")]
        [DataRow("test")]
        public void TestIsValidFileName(string fileName)
        {
            Assert.IsTrue(fileName.IsValidFileName());
        }

        /// <summary>
        /// Defines the test method CreateValidFileName.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        [DataTestMethod]
        [DataRow("test?")]
        [DataRow(".test")]
        [DataRow("test.")]
        [DataRow("/test")]
        [DataRow("\\test")]
        [DataRow("test/")]
        [DataRow("test\\")]
        [DataRow("test\\test")]
        [DataRow("test")]
        public void CreateValidFileName(string fileName)
        {
            Assert.IsTrue(fileName.MakeValidFileName().IsValidFileName());
        }
    }
}
