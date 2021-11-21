using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ISynergy.Framework.IO.Abtractions.Analyzers;
using ISynergy.Framework.IO.Analyzers;

namespace ISynergy.Framework.IO.Tests
{
    public partial class FileTypeAnalyzerTests
    {
        private readonly IFileTypeAnalyzer _fileTypeAnalyzer;

        public FileTypeAnalyzerTests()
        {
            _fileTypeAnalyzer = new FileTypeAnalyzer();
        }

        [TestMethod]
        public void CanDetectAlias_Jpg()
        {
            var filePath = GetFileByType("JPG");
            var fileContents = File.ReadAllBytes(filePath);

            var result = _fileTypeAnalyzer.IsType(fileContents, "jpg");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDetectAlias_Jpeg()
        {
            var filePath = GetFileByType("JPG");
            var fileContents = File.ReadAllBytes(filePath);

            var result = _fileTypeAnalyzer.IsType(fileContents, "jpeg");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDetectJpg_By_MimeType()
        {
            var filePath = GetFileByType("JPG");
            var fileContents = File.ReadAllBytes(filePath);

            var result = _fileTypeAnalyzer.IsType(fileContents, "image/jpeg");

            Assert.IsTrue(result);
        }

    }
}
