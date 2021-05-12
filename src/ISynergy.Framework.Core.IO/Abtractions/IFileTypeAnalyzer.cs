using ISynergy.Framework.Core.IO.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ISynergy.Framework.Core.IO.Abtractions
{
    /// <summary>
    /// Interface IFileAnalyzer
    /// </summary>
    public interface IFileTypeAnalyzer
    {
        /// <summary>
        /// Retrieve extensions that are supported based on the current definitions.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetAvailableExtensions();

        /// <summary>
        /// Retrieve mime types that are supported based on the current definitions.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetAvailableMimeTypes();

        /// <summary>
        /// Gets the MIME type by extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>string.</returns>
        string GetMimeTypeByExtension(string extension);

        /// <summary>
        /// Retrieve available types that are supported based on the current definitions.
        /// </summary>
        /// <value>The available types.</value>
        IEnumerable<FileTypeInfo> AvailableTypes { get; }

        /// <summary>
        /// Detect the file type.
        /// </summary>
        /// <param name="fileContent">The file contents to check.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>FileTypeInfo.</returns>
        FileTypeInfo DetectType(byte[] fileContent, string extension);

        /// <summary>
        /// Detect the file type.
        /// </summary>
        /// <param name="inputStream">Input stream to detect file type, if the stream is seekable the stream will be reset upon detecting.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>FileTypeInfo.</returns>
        FileTypeInfo DetectType(Stream inputStream, string extension);

        /// <summary>
        /// Determines if the file contents are of a specified type.
        /// </summary>
        /// <param name="fileContent">The file contents to examine.</param>
        /// <param name="extensionAliasOrMimeType">The mime- of file type to validate.</param>
        /// <returns><c>true</c> if the specified file content is type; otherwise, <c>false</c>.</returns>
        bool IsType(byte[] fileContent, string extensionAliasOrMimeType);
    }
}
