using ISynergy.Framework.Core.IO.Models;
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
        /// <summary>
        /// Defines the test method CanDetectAsciiTest.
        /// </summary>
        [Fact]
        public void CanDetectAsciiTest()
        {
            const string extension = "ascii";
            DetectType(extension, result =>
            {
                Assert.NotNull(result);
                Assert.StartsWith(extension, result.Name, StringComparison.OrdinalIgnoreCase);
            });
        }

        /// <summary>
        /// Defines the test method CanDetectUTF8Test.
        /// </summary>
        [Fact]
        public void CanDetectUTF8Test()
        {
            const string extension = "utf8";
            DetectType(extension, result =>
            {
                Assert.NotNull(result);
                Assert.StartsWith("UTF-8", result.Name, StringComparison.OrdinalIgnoreCase);
                Assert.True(result.Name.IndexOf("BOM", StringComparison.OrdinalIgnoreCase) == -1);
            });
        }

        /// <summary>
        /// Defines the test method CanDetectUTF8BOMTest.
        /// </summary>
        [Fact]
        public void CanDetectUTF8BOMTest()
        {
            const string extension = "utf8bom";
            DetectType(extension, result =>
            {
                Assert.NotNull(result);
                Assert.StartsWith("UTF-8", result.Name, StringComparison.OrdinalIgnoreCase);
                Assert.True(result.Name.IndexOf("BOM", StringComparison.OrdinalIgnoreCase) > -1);
            });
        }

        /// <summary>
        /// Defines the test method CanDetectAdobeTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [Theory]
        [InlineData("PDF")]
        [InlineData("FDF")]
        public void CanDetectAdobeTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectImagesTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [Theory]
        [InlineData("BMP")]
        [InlineData("GIF")]
        [InlineData("ICO")]
        [InlineData("JP2")]
        [InlineData("JPG")]
        [InlineData("PNG")]
        [InlineData("PSD")]
        [InlineData("TIF")]
        public void CanDetectImagesTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectVideoTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [Theory]
        [InlineData("3GP")]
        [InlineData("AVI")]
        [InlineData("FLV")]
        [InlineData("MID")]
        [InlineData("MP4")]
        [InlineData("WMV")]
        public void CanDetectVideoTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectAudioTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [Theory]
        [InlineData("AC3")]
        [InlineData("AIFF")]
        [InlineData("FLAC")]
        [InlineData("MP3")]
        [InlineData("OGG")]
        [InlineData("RA")]
        public void CanDetectAudioTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectOfficeTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [Theory]
        [InlineData("DOC")]
        [InlineData("DOCX")]
        [InlineData("PPT")]
        [InlineData("PPTX")]
        [InlineData("XLS")]
        [InlineData("XLSX")]
        public void CanDetectOfficeTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectFontTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [Theory]
        [InlineData("OTF")]
        [InlineData("TTF")]
        [InlineData("WOFF")]
        public void CanDetectFontTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectCompressedTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [Theory]
        [InlineData("7Z")]
        [InlineData("RAR")]
        [InlineData("ZIP")]
        public void CanDetectCompressedTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectXmlFileTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [Theory]
        [InlineData("XML")]
        public void CanDetectXmlFileTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectEmailFileTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [Theory]
        [InlineData("MSG")]
        public void CanDetectEmailFileTest(string extension)
        {
            DetectType(extension);
        }

        [Theory]
        [InlineData("XML", "application/xml")]
        [InlineData("ZIP", "application/zip")]
        [InlineData("DOC", "application/msword")]
        [InlineData("DOCX", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        [InlineData("PPT", "application/vnd.ms-powerpoint")]
        [InlineData("PPTX", "application/vnd.openxmlformats-officedocument.presentationml.presentation")]
        [InlineData("XLS", "application/vnd.ms-excel")]
        [InlineData("XLSX", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [InlineData("PDF", "application/pdf")]
        [InlineData("FDF", "application/vnd.fdf")]
        [InlineData("BMP", "image/bmp")]
        [InlineData("GIF", "image/gif")]
        [InlineData("ICO", "image/vnd.microsoft.icon")]
        [InlineData("JP2", "image/jp2")]
        [InlineData("JPG", "image/jpeg")]
        [InlineData("JPEG", "image/jpeg")]
        [InlineData("PNG", "image/png")]
        [InlineData("PSD", "image/vnd.adobe.photoshop")]
        [InlineData("TIF", "image/tiff")]
        public void GetMimeTypeByExtensionTest(string extension, string mimeType)
        {
            Assert.Equal(mimeType, _fileTypeAnalyzer.GetMimeTypeByExtension(extension));
        }

        /// <summary>
        /// Detects the type.
        /// </summary>
        /// <param name="extension">The extension.</param>
        private void DetectType(string extension)
        {
            DetectType(extension, result =>
            {
                Assert.NotNull(result);
                Assert.True(
                    result.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase) ||
                    result.Aliases?.Any(a => a.Equals(extension, StringComparison.OrdinalIgnoreCase)) == true);
            });
        }

        /// <summary>
        /// Detects the type.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="assertionValidator">The assertion validator.</param>
        /// <exception cref="FileNotFoundException">Testfile with {extension} extension is not found.</exception>
        private void DetectType(string extension, Action<FileTypeInfo> assertionValidator)
        {
            var files = GetFilesByExtension(extension);

            if (files is null || files.Count() == 0)
                throw new FileNotFoundException($"Testfile with {extension} extension is not found.");

            foreach (var file in files)
            {
                var fileContents = File.ReadAllBytes(file);
                var result = _fileTypeAnalyzer.DetectType(fileContents, extension);
                assertionValidator(result);
            }
        }

        /// <summary>
        /// Gets the type of the file by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        private string GetFileByType(string type) =>
            Path.Combine(GetTestFileDirectory(), $"{type}.{type}");


        /// <summary>
        /// GetFiles with searchPattern returns 4 character extensions when
        /// filtering for 3 so we'll filter ourselves
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        private IEnumerable<string> GetFilesByExtension(string type) =>
            Directory.GetFiles(GetTestFileDirectory(), $"*.{type}")
                .Where(path => path.EndsWith(type, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Gets the test file directory.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetTestFileDirectory() =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles");
    }
}
