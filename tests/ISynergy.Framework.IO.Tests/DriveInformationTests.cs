using ISynergy.Framework.IO.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.IO.Tests;

/// <summary>
/// Class DriveInformationTests.
/// </summary>
[TestClass]
public class DriveInformationTests
{
    /// <summary>
    /// Defines the test method IsNetworkDriveTest.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="result">The result.</param>
    [DataTestMethod]
    [DataRow(@"C:\Temp", 0)]
    [DataRow(@"\\localhost\c$\temp", 1)]
    [DataRow(@"\\SERVER\Temp", 1)]
    //[DataRow(@"Z:", 1)] // Mapped drive pointing to \\workstation\Temp
    [DataRow(@"C:\", 0)]
    [DataRow(@"Temp", -1)]
    [DataRow(@".\Temp", -1)]
    [DataRow(@"..\Temp", -1)]
    public void IsNetworkDriveTest(string path, int result)
    {
        // If result = -1, the expected outcome is an exception.
        // otherwise its a boolean value.
        if (result == -1)
        {
            Assert.Throws<ArgumentException>(() => DriveInformation.IsNetworkDrive(path));
        }
        else
        {
            Assert.AreEqual(Convert.ToBoolean(result), DriveInformation.IsNetworkDrive(path));
        }
    }

    /// <summary>
    /// Defines the test method GetDriveNameTest.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="result">The result.</param>
    [DataTestMethod]
    [DataRow(@"Z:\Temp\Sub-Folder", @"Z:\")]
    [DataRow(@"C:\Temp", @"C:\")]
    [DataRow(@"\\localhost\c$\temp", null)]
    [DataRow(@"\\SERVER\Temp", null)]
    [DataRow(@"Z:", @"Z:\")] // Mapped drive pointing to \\workstation\Temp
    [DataRow(@"C:\", @"C:\")]
    [DataRow(@"Temp", null)]
    [DataRow(@".\Temp", null)]
    [DataRow(@"..\Temp", null)]
    public void GetDriveNameTest(string path, object result)
    {
        // If result is null, the expected outcome is an exception.
        // otherwise its a string value with the drive name.
        if (result is null)
        {
            Assert.Throws<ArgumentException>(() => DriveInformation.GetDriveName(path));
        }
        else
        {
            Assert.AreEqual(result, DriveInformation.GetDriveName(path));
        }
    }

    /// <summary>
    /// Defines the test method ResolveToRootUNCTest.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="result">The result.</param>
    [DataTestMethod]
    [DataRow(@"Z:\Temp\Sub-Folder", @"Z:\")]
    [DataRow(@"C:\Temp", @"C:\")]
    [DataRow(@"\\localhost\c$\temp", @"\\localhost\c$")]
    [DataRow(@"\\SERVER\Temp", @"\\SERVER\Temp")]
    [DataRow(@"Z:", @"Z:\")] // Mapped drive pointing to \\workstation\Temp
    [DataRow(@"C:\", @"C:\")]
    [DataRow(@"Temp", null)]
    [DataRow(@".\Temp", null)]
    [DataRow(@"..\Temp", null)]
    public void ResolveToRootUNCTest(string path, object result)
    {
        // If result is null, the expected outcome is an exception.
        // otherwise its a string value with the drive name.
        if (result is null)
        {
            Assert.Throws<ArgumentException>(() => DriveInformation.ResolveToRootUNC(path));
        }
        else
        {
            Assert.AreEqual(result, DriveInformation.ResolveToRootUNC(path));
        }
    }

    /// <summary>
    /// Defines the test method ResolveToUNCTest.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="result">The result.</param>
    [DataTestMethod]
    [DataRow(@"Z:\Temp\Sub-Folder", @"Z:\Temp\Sub-Folder")]
    [DataRow(@"C:\Temp", @"C:\Temp")]
    [DataRow(@"\\localhost\c$\temp", @"\\localhost\c$\temp")]
    [DataRow(@"\\SERVER\Temp", @"\\SERVER\Temp")]
    [DataRow(@"Z:", @"Z:")] // Mapped drive pointing to \\workstation\Temp
    [DataRow(@"C:\", @"C:\")]
    [DataRow(@"Temp", null)]
    [DataRow(@".\Temp", null)]
    [DataRow(@"..\Temp", null)]
    public void ResolveToUNCTest(string path, object result)
    {
        // If result is null, the expected outcome is an exception.
        // otherwise its a string value with the drive name.
        if (result is null)
        {
            Assert.Throws<ArgumentException>(() => DriveInformation.ResolveToUNC(path));
        }
        else
        {
            Assert.AreEqual(result, DriveInformation.ResolveToUNC(path));
        }
    }

    /// <summary>
    /// Defines the test method IsFreeSpaceAvailableTest.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="result">The result.</param>
    [DataTestMethod]
    //[DataRow(@"Z:\Temp\Sub-Folder", true)]
    [DataRow(@"C:\Temp", true)]
    [DataRow(@"\\localhost\c$\temp", true)]
    [DataRow(@"\\SERVER\Temp", true)]
    //[DataRow(@"Z:", true)] // Mapped drive pointing to \\workstation\Temp
    [DataRow(@"C:\", true)]
    [DataRow(@"Temp", null)]
    [DataRow(@".\Temp", null)]
    [DataRow(@"..\Temp", null)]
    public void IsFreeSpaceAvailableTest(string path, object result)
    {
        long fileSize = 1L;

        // If result is null, the expected outcome is an exception.
        // otherwise its a string value with the drive name.
        if (result is null)
        {
            Assert.Throws<ArgumentException>(() => DriveInformation.IsFreeSpaceAvailable(path, fileSize));
        }
        else
        {
            Assert.AreEqual(Convert.ToBoolean(result), DriveInformation.IsFreeSpaceAvailable(path, fileSize));
        }
    }
}
