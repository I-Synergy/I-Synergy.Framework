using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mvvm.Models.Tests;

[TestClass]
public class FileResultTests
{
    private const string TestFilePath = @"C:\test\file.txt";
    private const string TestFileName = "file.txt";
    private byte[] _testData;
    private MemoryStream _testStream;

    [TestInitialize]
    public void Setup()
    {
        _testData = new byte[] { 1, 2, 3, 4, 5 };
        _testStream = new MemoryStream(_testData);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _testStream?.Dispose();
    }

    [TestMethod]
    public void Constructor_InitializesProperties()
    {
        // Arrange & Act
        var fileResult = new FileResult(TestFilePath, TestFileName, () => _testStream);

        // Assert
        Assert.AreEqual(TestFilePath, fileResult.FilePath);
        Assert.AreEqual(TestFileName, fileResult.FileName);
    }

    [TestMethod]
    public void GetStream_ReturnsValidStream()
    {
        // Arrange
        var fileResult = new FileResult(TestFilePath, TestFileName, () => _testStream);

        // Act
        using var stream = fileResult.GetStream();

        // Assert
        Assert.IsNotNull(stream);
        Assert.IsTrue(stream.CanRead);
    }

    [TestMethod]
    public void File_ReturnsCorrectData()
    {
        // Arrange
        var fileResult = new FileResult(TestFilePath, TestFileName, () => new MemoryStream(_testData));

        // Act
        var resultData = fileResult.File;

        // Assert
        CollectionAssert.AreEqual(_testData, resultData);
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void GetStream_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var fileResult = new FileResult(TestFilePath, TestFileName, () => _testStream);
        fileResult.Dispose();

        // Act
        fileResult.GetStream();
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void FileName_Get_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var fileResult = new FileResult(TestFilePath, TestFileName, () => _testStream);
        fileResult.Dispose();

        // Act
        _ = fileResult.FileName;
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void FileName_Set_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var fileResult = new FileResult(TestFilePath, TestFileName, () => _testStream);
        fileResult.Dispose();

        // Act
        fileResult.FileName = "new.txt";
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void FilePath_Get_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var fileResult = new FileResult(TestFilePath, TestFileName, () => _testStream);
        fileResult.Dispose();

        // Act
        _ = fileResult.FilePath;
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void FilePath_Set_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var fileResult = new FileResult(TestFilePath, TestFileName, () => _testStream);
        fileResult.Dispose();

        // Act
        fileResult.FilePath = @"C:\new\path.txt";
    }

    [TestMethod]
    public void ReadFully_ReturnsCorrectData()
    {
        // Arrange
        using var stream = new MemoryStream(_testData);

        // Act
        var result = FileResult.ReadFully(stream);

        // Assert
        CollectionAssert.AreEqual(_testData, result);
    }

    [TestMethod]
    public void Dispose_CallsDisposeAction()
    {
        // Arrange
        bool disposeCalled = false;
        var fileResult = new FileResult(TestFilePath, TestFileName, () => _testStream,
            disposing => disposeCalled = true);

        // Act
        fileResult.Dispose();

        // Assert
        Assert.IsTrue(disposeCalled);
    }

    [TestMethod]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        int disposeCount = 0;
        var fileResult = new FileResult(TestFilePath, TestFileName, () => _testStream,
            disposing => disposeCount++);

        // Act
        fileResult.Dispose();
        fileResult.Dispose();

        // Assert
        Assert.AreEqual(1, disposeCount, "Dispose should only execute once");
    }

    [TestMethod]
    public void Dispose_SetsIsDisposedFlag()
    {
        // Arrange
        var fileResult = new FileResult(TestFilePath, TestFileName, () => _testStream);

        // Act
        fileResult.Dispose();

        // Assert - verify through property access exceptions
        Assert.ThrowsException<ObjectDisposedException>(() => fileResult.GetStream());
    }
}
