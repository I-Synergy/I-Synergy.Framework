using System;
using Xunit;

namespace ISynergy.Framework.Core.IO.Tests
{
    public class DriveInformationTests
    {
        [Theory]
        [InlineData(@"C:\Temp", 0)]
        [InlineData(@"\\localhost\c$\temp", 1)]
        [InlineData(@"\\SERVER\Temp", 1)]
        //[InlineData(@"Z:", 1)] // Mapped drive pointing to \\workstation\Temp
        [InlineData(@"C:\", 0)]
        [InlineData(@"Temp", -1)]
        [InlineData(@".\Temp", -1)]
        [InlineData(@"..\Temp", -1)]
        public void IsNetworkDriveTest(string path, int result)
        {
            // If result = -1, the expected outcome is an exception.
            // otherwise its a boolean value.
            if(result == -1)
            {
                Assert.Throws<ArgumentException>(() => DriveInformation.IsNetworkDrive(path));
            }
            else
            {
                Assert.Equal(Convert.ToBoolean(result), DriveInformation.IsNetworkDrive(path));
            }
        }

        [Theory]
        [InlineData(@"Z:\Temp\Sub-Folder", @"Z:\")]
        [InlineData(@"C:\Temp", @"C:\")]
        [InlineData(@"\\localhost\c$\temp", null)]
        [InlineData(@"\\SERVER\Temp", null)]
        [InlineData(@"Z:", @"Z:\")] // Mapped drive pointing to \\workstation\Temp
        [InlineData(@"C:\", @"C:\")]
        [InlineData(@"Temp", null)]
        [InlineData(@".\Temp", null)]
        [InlineData(@"..\Temp", null)]
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
                Assert.Equal(result, DriveInformation.GetDriveName(path));
            }
        }

        [Theory]
        [InlineData(@"Z:\Temp\Sub-Folder", @"Z:\")]
        [InlineData(@"C:\Temp", @"C:\")]
        [InlineData(@"\\localhost\c$\temp", @"\\localhost\c$")]
        [InlineData(@"\\SERVER\Temp", @"\\SERVER\Temp")]
        [InlineData(@"Z:", @"Z:\")] // Mapped drive pointing to \\workstation\Temp
        [InlineData(@"C:\", @"C:\")]
        [InlineData(@"Temp", null)]
        [InlineData(@".\Temp", null)]
        [InlineData(@"..\Temp", null)]
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
                Assert.Equal(result, DriveInformation.ResolveToRootUNC(path));
            }
        }

        [Theory]
        [InlineData(@"Z:\Temp\Sub-Folder", @"Z:\Temp\Sub-Folder")]
        [InlineData(@"C:\Temp", @"C:\Temp")]
        [InlineData(@"\\localhost\c$\temp", @"\\localhost\c$\temp")]
        [InlineData(@"\\SERVER\Temp", @"\\SERVER\Temp")]
        [InlineData(@"Z:", @"Z:")] // Mapped drive pointing to \\workstation\Temp
        [InlineData(@"C:\", @"C:\")]
        [InlineData(@"Temp", null)]
        [InlineData(@".\Temp", null)]
        [InlineData(@"..\Temp", null)]
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
                Assert.Equal(result, DriveInformation.ResolveToUNC(path));
            }
        }

        [Theory]
        //[InlineData(@"Z:\Temp\Sub-Folder", true)]
        [InlineData(@"C:\Temp", true)]
        [InlineData(@"\\localhost\c$\temp", true)]
        [InlineData(@"\\SERVER\Temp", true)]
        //[InlineData(@"Z:", true)] // Mapped drive pointing to \\workstation\Temp
        [InlineData(@"C:\", true)]
        [InlineData(@"Temp", null)]
        [InlineData(@".\Temp", null)]
        [InlineData(@"..\Temp", null)]
        public void IsFreeSpaceAvailableTest(string path, object result)
        {
            var fileSize = 1L;

            // If result is null, the expected outcome is an exception.
            // otherwise its a string value with the drive name.
            if (result is null)
            {
                Assert.Throws<ArgumentException>(() => DriveInformation.IsFreeSpaceAvailable(path, fileSize));
            }
            else
            {
                Assert.Equal(Convert.ToBoolean(result), DriveInformation.IsFreeSpaceAvailable(path, fileSize));
            }
        }
    }
}
