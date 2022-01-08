using System;
using System.IO;

namespace ISynergy.Framework.Synchronization.Core.Models
{
    /// <summary>
    /// File info with meta data class
    /// </summary>
    public sealed class FileInfoMetadata : IEquatable<FileInfoMetadata>
    {
        /// <summary>
        /// Relative file path name.
        /// </summary>
        public string RelativePathName { get; set; }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the fullname of the file.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets the size of the file. in bytes.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FileInfoMetadata() { }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="fileInfo"></param>
        public FileInfoMetadata(string rootPath, FileInfo fileInfo)
        {
            RelativePathName = fileInfo.FullName.Substring(rootPath.Length);
            Name = fileInfo.Name;
            FullName = fileInfo.FullName;
            Length = fileInfo.Length;
        }

        /// <summary>
        /// Checks for equality of metadata.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(FileInfoMetadata other) =>
            (RelativePathName == other.RelativePathName) && (Length == other.Length);
    }
}
