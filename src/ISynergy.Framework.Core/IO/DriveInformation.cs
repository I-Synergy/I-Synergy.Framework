using System;
using System.IO;
using System.Linq;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.IO
{
    /// <summary>
    /// Class DriveInformation.
    /// </summary>
    public static class DriveInformation
    {
        /// <summary>
        /// Determines whether free space is available on the specified path.
        /// If path is a network URI then result is always true.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileSize">Size of the file.</param>
        /// <returns><c>true</c> if [is free space available] [the specified path]; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The path '{path}' was not a rooted path and IsFreeSpaceAvailable does not support relative paths.</exception>
        public static bool IsFreeSpaceAvailable(string path, long fileSize)
        {
            Argument.IsNotNullOrEmpty(nameof(path), path);
            Argument.IsMinimal(nameof(fileSize), fileSize, 1);

            if (!Path.IsPathRooted(path))
            {
                throw new ArgumentException($"The path '{path}' was not a rooted path and IsFreeSpaceAvailable does not support relative paths.");
            }

            if (path.StartsWith(@"\\"))
            {
                return true;
            }

            // Get just the drive letter for WMI call
            string driveletter = GetDriveName(path);
            var drive = DriveInfo.GetDrives().Where(q => q.Name == driveletter).SingleOrDefault();

            return drive.TotalFreeSpace > fileSize;
        }

        /// <summary>
        /// Determines whether the specified path is a network drive.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if [is network drive] [the specified path]; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The path '{path}' was not a rooted path.</exception>
        public static bool IsNetworkDrive(string path)
        {
            Argument.IsNotNullOrEmpty(nameof(path), path);

            if (!Path.IsPathRooted(path))
                throw new ArgumentException($"The path '{path}' was not a rooted path.");

            if (path.StartsWith(@"\\"))
                return true;

            // Get just the drive letter for WMI call
            string driveletter = GetDriveName(path);
            
            var drive = DriveInfo.GetDrives().Where(q => q.Name == driveletter).SingleOrDefault();

            if(drive != null && drive.DriveType == DriveType.Network)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the name of the drive.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">The path '{path}' was not a rooted path.</exception>
        /// <exception cref="ArgumentException">A UNC path was passed to GetDriveName</exception>
        /// <exception cref="ArgumentException">The path '{path}' was not a rooted path.</exception>
        /// <exception cref="ArgumentException">A UNC path was passed to GetDriveName</exception>
        public static string GetDriveName(string path)
        {
            Argument.IsNotNullOrEmpty(nameof(path), path);

            if (!Path.IsPathRooted(path))
                throw new ArgumentException($"The path '{path}' was not a rooted path.");

            if (path.StartsWith(@"\\"))
                throw new ArgumentException("A UNC path was passed to GetDriveName");

            return Directory.GetDirectoryRoot(path);
        }

        /// <summary>
        /// Resolves the given path to a root UNC path if the path is a mapped drive.
        /// Otherwise, just returns the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">The path '{path}' was not a rooted path and ResolveToRootUNC does not support relative paths.</exception>
        public static string ResolveToRootUNC(string path)
        {
            Argument.IsNotNullOrEmpty(nameof(path), path);

            if (!Path.IsPathRooted(path))
            {
                throw new ArgumentException($"The path '{path}' was not a rooted path and ResolveToRootUNC does not support relative paths.");
            }

            if (path.StartsWith(@"\\"))
            {
                return Directory.GetDirectoryRoot(path);
            }

            // Get just the drive letter for WMI call
            string driveletter = GetDriveName(path);

            var drive = DriveInfo.GetDrives().Where(q => q.Name == driveletter).SingleOrDefault();

            if (drive != null && drive.DriveType == DriveType.Network)
            {
                return drive.RootDirectory.ToString();
            }
            else
            {
                return driveletter;
            }
        }

        /// <summary>
        /// Resolves the given path to a full UNC path if the path is a mapped drive.
        /// Otherwise, just returns the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">The path '{path}' was not a rooted path and ResolveToUNC does not support relative paths.</exception>
        public static string ResolveToUNC(string path)
        {
            Argument.IsNotNullOrEmpty(nameof(path), path);

            if (!Path.IsPathRooted(path))
            {
                throw new ArgumentException($"The path '{path}' was not a rooted path and ResolveToUNC does not support relative paths.");
            }

            // Is the path already in the UNC format?
            if (path.StartsWith(@"\\"))
            {
                return path;
            }

            string rootPath = ResolveToRootUNC(path);

            if (path.StartsWith(rootPath))
            {
                return path; // Local drive, no resolving occurred
            }
            else
            {
                return path.Replace(GetDriveName(path), rootPath);
            }
        }
    }
}
