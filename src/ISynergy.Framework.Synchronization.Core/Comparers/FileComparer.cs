using ISynergy.Framework.Synchronization.Core.Models;
using System.Collections.Generic;

namespace ISynergy.Framework.Synchronization.Core.Comparers
{
    /// <summary>
    /// Meta data comparer class.
    /// </summary>
    internal class FileComparer : IEqualityComparer<FileInfoMetadata>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FileComparer() { }

        /// <summary>
        /// Checks if fileInfoMetadata1 equals to fileInfoMetadata2.
        /// </summary>
        /// <param name="fileInfoMetadata1"></param>
        /// <param name="fileInfoMetadata2"></param>
        /// <returns></returns>
        public bool Equals(FileInfoMetadata fileInfoMetadata1, FileInfoMetadata fileInfoMetadata2) =>
            fileInfoMetadata1.Name == fileInfoMetadata2.Name && 
            fileInfoMetadata1.Length == fileInfoMetadata2.Length;

        /// <summary>
        /// Return a hash that reflects the comparison criteria.
        /// According to the rules for IEqualityComparer{T}, if Equals is true, then the hash codes must also be equal.
        /// Because equality as defined here is a simple value equality, not reference identity, it is possible that two or more objects will produce the same hash code.  
        /// </summary>
        /// <param name="fileInfoMetadata"></param>
        /// <returns></returns>
        public int GetHashCode(FileInfoMetadata fileInfoMetadata) =>
            ($"{fileInfoMetadata.RelativePathName}{fileInfoMetadata.Length}").GetHashCode();
    }
}
