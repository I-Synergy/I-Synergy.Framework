using ISynergy.Framework.IO.Abtractions.Analyzers;
using ISynergy.Framework.IO.Analyzers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.IO.Tests;

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
        string filePath = GetFileByType("JPG");
        byte[] fileContents = File.ReadAllBytes(filePath);

        bool result = _fileTypeAnalyzer.IsType(fileContents, "jpg");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CanDetectAlias_Jpeg()
    {
        string filePath = GetFileByType("JPG");
        byte[] fileContents = File.ReadAllBytes(filePath);

        bool result = _fileTypeAnalyzer.IsType(fileContents, "jpeg");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CanDetectJpg_By_MimeType()
    {
        string filePath = GetFileByType("JPG");
        byte[] fileContents = File.ReadAllBytes(filePath);

        bool result = _fileTypeAnalyzer.IsType(fileContents, "image/jpeg");

        Assert.IsTrue(result);
    }

}
