using ISynergy.Framework.IO.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ISynergy.Framework.IO.Tests
{
    [TestClass]
    public partial class FileTypeAnalyzerTests
    {
        /// <summary>
        /// Defines the test method CanDetectAsciiTest.
        /// </summary>
        [TestMethod]
        public void CanDetectAsciiTest()
        {
            const string extension = "ascii";
            DetectType(extension, result =>
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Name.StartsWith(extension, StringComparison.OrdinalIgnoreCase));
            });
        }

        /// <summary>
        /// Defines the test method CanDetectUTF8Test.
        /// </summary>
        [TestMethod]
        public void CanDetectUTF8Test()
        {
            const string extension = "utf8";
            DetectType(extension, result =>
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Name.StartsWith("UTF-8", StringComparison.OrdinalIgnoreCase));
                Assert.IsTrue(result.Name.IndexOf("BOM", StringComparison.OrdinalIgnoreCase) == -1);
            });
        }

        /// <summary>
        /// Defines the test method CanDetectUTF8BOMTest.
        /// </summary>
        [TestMethod]
        public void CanDetectUTF8BOMTest()
        {
            const string extension = "utf8bom";
            DetectType(extension, result =>
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Name.StartsWith("UTF-8", StringComparison.OrdinalIgnoreCase));
                Assert.IsTrue(result.Name.IndexOf("BOM", StringComparison.OrdinalIgnoreCase) > -1);
            });
        }

        /// <summary>
        /// Defines the test method CanDetectAdobeTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [DataTestMethod]
        [DataRow("PDF")]
        [DataRow("FDF")]
        public void CanDetectAdobeTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectImagesTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [DataTestMethod]
        [DataRow("BMP")]
        [DataRow("GIF")]
        [DataRow("ICO")]
        [DataRow("JP2")]
        [DataRow("JPG")]
        [DataRow("PNG")]
        [DataRow("PSD")]
        [DataRow("TIF")]
        public void CanDetectImagesTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectVideoTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [DataTestMethod]
        [DataRow("3GP")]
        [DataRow("AVI")]
        [DataRow("FLV")]
        [DataRow("MID")]
        [DataRow("MP4")]
        [DataRow("WMV")]
        public void CanDetectVideoTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectAudioTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [DataTestMethod]
        [DataRow("AC3")]
        [DataRow("AIFF")]
        [DataRow("FLAC")]
        [DataRow("MP3")]
        [DataRow("OGG")]
        [DataRow("RA")]
        public void CanDetectAudioTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectOfficeTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [DataTestMethod]
        [DataRow("DOC")]
        [DataRow("DOCX")]
        [DataRow("PPT")]
        [DataRow("PPTX")]
        [DataRow("XLS")]
        [DataRow("XLSX")]
        public void CanDetectOfficeTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectFontTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [DataTestMethod]
        [DataRow("OTF")]
        [DataRow("TTF")]
        [DataRow("WOFF")]
        public void CanDetectFontTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectCompressedTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [DataTestMethod]
        [DataRow("7Z")]
        [DataRow("RAR")]
        [DataRow("ZIP")]
        public void CanDetectCompressedTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectXmlFileTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [DataTestMethod]
        [DataRow("XML")]
        public void CanDetectXmlFileTest(string extension)
        {
            DetectType(extension);
        }

        /// <summary>
        /// Defines the test method CanDetectEmailFileTest.
        /// </summary>
        /// <param name="extension">The extension.</param>
        [DataTestMethod]
        [DataRow("MSG")]
        public void CanDetectEmailFileTest(string extension)
        {
            DetectType(extension);
        }

        [DataTestMethod]
        [DataRow("XML", "application/xml")]
        [DataRow("ZIP", "application/zip")]
        [DataRow("DOC", "application/msword")]
        [DataRow("DOCX", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        [DataRow("PPT", "application/vnd.ms-powerpoint")]
        [DataRow("PPTX", "application/vnd.openxmlformats-officedocument.presentationml.presentation")]
        [DataRow("XLS", "application/vnd.ms-excel")]
        [DataRow("XLSX", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [DataRow("PDF", "application/pdf")]
        [DataRow("FDF", "application/vnd.fdf")]
        [DataRow("BMP", "image/bmp")]
        [DataRow("GIF", "image/gif")]
        [DataRow("ICO", "image/vnd.microsoft.icon")]
        [DataRow("JP2", "image/jp2")]
        [DataRow("JPG", "image/jpeg")]
        [DataRow("JPEG", "image/jpeg")]
        [DataRow("PNG", "image/png")]
        [DataRow("PSD", "image/vnd.adobe.photoshop")]
        [DataRow("TIF", "image/tiff")]
        public void GetMimeTypeByExtensionTest(string extension, string mimeType)
        {
            Assert.AreEqual(mimeType, _fileTypeAnalyzer.GetMimeTypeByExtension(extension));
        }

        /// <summary>
        /// Detects the type.
        /// </summary>
        /// <param name="extension">The extension.</param>
        private void DetectType(string extension)
        {
            DetectType(extension, result =>
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(
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
            IEnumerable<string> files = GetFilesByExtension(extension);

            if (files is null || files.Count() == 0)
                throw new FileNotFoundException($"Testfile with {extension} extension is not found.");

            foreach (string file in files)
            {
                byte[] fileContents = File.ReadAllBytes(file);
                FileTypeInfo result = _fileTypeAnalyzer.DetectType(fileContents, extension);
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
