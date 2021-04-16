using ISynergy.Framework.Core.IO.Abtractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Framework.Core.IO.Tests
{
    public partial class FileTypeAnalyzerTests
    {
        private readonly IFileTypeAnalyzer _fileTypeAnalyzer;

        public FileTypeAnalyzerTests()
        {
            _fileTypeAnalyzer = new FileTypeAnalyzer();
        }

        [Fact]
        public void CanDetectAlias_Jpg()
        {
            var filePath = GetFileByType("JPG");
            var fileContents = File.ReadAllBytes(filePath);

            var result = _fileTypeAnalyzer.IsType(fileContents, "jpg");

            Assert.True(result);
        }

        [Fact]
        public void CanDetectAlias_Jpeg()
        {
            var filePath = GetFileByType("JPG");
            var fileContents = File.ReadAllBytes(filePath);

            var result = _fileTypeAnalyzer.IsType(fileContents, "jpeg");

            Assert.True(result);
        }

        [Fact]
        public void CanDetectJpg_By_MimeType()
        {
            var filePath = GetFileByType("JPG");
            var fileContents = File.ReadAllBytes(filePath);

            var result = _fileTypeAnalyzer.IsType(fileContents, "image/jpeg");

            Assert.True(result);
        }

    }
}
