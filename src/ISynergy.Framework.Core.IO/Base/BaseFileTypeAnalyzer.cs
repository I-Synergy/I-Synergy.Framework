using ISynergy.Framework.Core.IO.Abtractions;
using ISynergy.Framework.Core.IO.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ISynergy.Framework.Core.IO.Base
{
    /// <summary>
    /// Class BaseFileTypeAnalyzer.
    /// Implements the <see cref="IFileTypeAnalyzer" />
    /// </summary>
    /// <seealso cref="IFileTypeAnalyzer" />
    public class BaseFileTypeAnalyzer : IFileTypeAnalyzer
    {
        /// <summary>
        /// The lazy file types
        /// </summary>
        private readonly Lazy<IEnumerable<FileTypeInfo>> lazyFileTypes;
        /// <summary>
        /// The ASCII file type
        /// </summary>
        private readonly FileTypeInfo asciiFileType = new() { Name = "ASCII Text", Extension = "txt", MimeType = "text/plain", Signature = null };
        /// <summary>
        /// The UTF8 file type
        /// </summary>
        private readonly FileTypeInfo utf8FileType = new() { Name = "UTF-8 Text", Extension = "txt", MimeType = "text/plain", Signature = null };
        /// <summary>
        /// The UTF8 file type with bom
        /// </summary>
        private readonly FileTypeInfo utf8FileTypeWithBOM = new() { Name = "UTF-8 Text with BOM", Extension = "txt", MimeType = "text/plain", Signature = null };

        /// <summary>
        /// Initializes a <see cref="BaseFileTypeAnalyzer" /> with the provided json definition.
        /// </summary>
        /// <param name="jsonDefinition">The json definition file.</param>
        internal BaseFileTypeAnalyzer(string jsonDefinition)
        {
            lazyFileTypes = new Lazy<IEnumerable<FileTypeInfo>>(() => LoadFileTypes(jsonDefinition).ToList());
        }

        /// <summary>
        /// Retrieve available types that are supported based on the current definitions.
        /// </summary>
        /// <value>The available types.</value>
        public IEnumerable<FileTypeInfo> AvailableTypes => lazyFileTypes.Value;

        /// <summary>
        /// Detect the file type.
        /// </summary>
        /// <param name="inputStream">Input stream to detect file type, if the stream is seekable the stream will be reset upon detecting.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>FileTypeInfo.</returns>
        /// <exception cref="ArgumentNullException">inputStream</exception>
        public FileTypeInfo DetectType(Stream inputStream, string extension)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            if (inputStream.CanSeek)
                inputStream.Position = 0;

            byte[] byteBuffer = new byte[inputStream.Length];
            inputStream.Read(byteBuffer, 0, byteBuffer.Length);

            if (inputStream.CanSeek)
                inputStream.Position = 0;

            return DetectType(byteBuffer, extension);
        }

        /// <summary>
        /// Detect the file type.
        /// </summary>
        /// <param name="fileContent">The file contents to check.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>FileTypeInfo.</returns>
        /// <exception cref="ArgumentNullException">fileContent</exception>
        /// <exception cref="ArgumentException">input must not be empty</exception>
        public FileTypeInfo DetectType(byte[] fileContent, string extension)
        {
            if (fileContent == null)
                throw new ArgumentNullException(nameof(fileContent));

            if (fileContent.Length == 0)
                throw new ArgumentException("input must not be empty");

            // iterate over each type and determine if we have a match based on file signature.
            foreach (var fileTypeInfo in AvailableTypes
                .Where(q => 
                    q.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase) || 
                    (!string.IsNullOrEmpty(q.Alias) && q.Alias.IndexOf(extension, 0, StringComparison.OrdinalIgnoreCase) != -1))
                .ToList())
            {
                // if we found a match return the matching filetypeinfo
                if (IsMatchingType(fileContent, fileTypeInfo))
                    return fileTypeInfo;
            }

            if (IsAscii(fileContent))
                return asciiFileType;

            if (IsUTF8(fileContent, out bool hasBOM))
                return hasBOM ? utf8FileTypeWithBOM : utf8FileType;

            return null;
        }

        /// <summary>
        /// Retrieve extensions that are supported based on the current definitions.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetAvailableExtensions() =>
            AvailableTypes.Select(t => t.Extension).Distinct();

        /// <summary>
        /// Retrieve mime types that are supported based on the current definitions.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetAvailableMimeTypes() =>
            AvailableTypes.Select(t => t.MimeType).Distinct();

        /// <summary>
        /// Gets the MIME type by extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>System.String.</returns>
        public string GetMimeTypeByExtension(string extension) =>
            AvailableTypes
                .Where(q => 
                    q.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase) || 
                    q.Alias.IndexOf(extension, 0, StringComparison.OrdinalIgnoreCase) != -1)
                .Select(s => s.MimeType)
                .FirstOrDefault();

        /// <summary>
        /// Determines if the file contents are of a specified type.
        /// </summary>
        /// <param name="fileContent">The file contents to examine.</param>
        /// <param name="extensionAliasOrMimeType">The file type to validate.</param>
        /// <returns><c>true</c> if the specified file content is type; otherwise, <c>false</c>.</returns>
        public bool IsType(byte[] fileContent, string extensionAliasOrMimeType)
        {
            foreach (var fileTypeInfo in AvailableTypes.Where(t =>
                t.Extension.Equals(extensionAliasOrMimeType, StringComparison.OrdinalIgnoreCase) ||
                t.MimeType.Equals(extensionAliasOrMimeType, StringComparison.OrdinalIgnoreCase) ||
                (t.Aliases != null && t.Aliases.Contains(extensionAliasOrMimeType, StringComparer.OrdinalIgnoreCase))))
            {
                if (IsMatchingType(fileContent, fileTypeInfo))
                    return true;
            }

            if (extensionAliasOrMimeType.Equals("txt", StringComparison.OrdinalIgnoreCase) ||
                extensionAliasOrMimeType.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
                return IsText(fileContent, out bool hasBOM);

            return false;
        }

        /// <summary>
        /// Determines whether [is matching type] [the specified input].
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is matching type] [the specified input]; otherwise, <c>false</c>.</returns>
        private static bool IsMatchingType(IList<byte> input, FileTypeInfo type)
        {
            // find an initial match based on the header and offset
            var isMatch = FindMatch(input, type.Header, type.Offset);

            // some file types have the same header
            // but different signature in another location, if its one of these determine what the true file type is
            if (isMatch && type.SubHeader != null)
            {
                // find all indices of matching the 1st byte of the additional sequence
                var matchingIndices = new List<int>();
                for (int i = 0; i < input.Count; i++)
                {
                    if (input[i] == type.SubHeader[0])
                        matchingIndices.Add(i);
                }

                // investigate all of them for a match
                foreach (int potentialMatchingIndex in matchingIndices)
                {
                    isMatch = FindMatch(input, type.SubHeader, potentialMatchingIndex);

                    if (isMatch)
                        break;
                }
            }

            return isMatch;
        }

        /// <summary>
        /// Finds the match.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="searchArray">The search array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool FindMatch(IList<byte> input, IList<byte> searchArray, int offset = 0)
        {
            // file isn't long enough to even search the proper index, not a match
            if (input.Count <= offset)
                return false;

            int matchingCount = 0;
            for (var i = 0; i < searchArray.Count; i++)
            {
                // set the offset location
                var calculatedOffset = i + offset;

                if (input.Count <= calculatedOffset)
                    break;

                // if file offset is not set to zero, we need to take this into account when comparing.
                // if byte in searchArray is set to null, means this byte is variable, ignore it
                if (searchArray[i] != input[calculatedOffset])
                {
                    // if one of the bytes do not match, move on
                    matchingCount = 0;
                    break;
                }
                matchingCount++;
            }
            return matchingCount == searchArray.Count;
        }

        /// <summary>
        /// Loads the file types.
        /// </summary>
        /// <param name="flatFileData">The flat file data.</param>
        /// <returns>IEnumerable&lt;FileTypeInfo&gt;.</returns>
        private static IEnumerable<FileTypeInfo> LoadFileTypes(string flatFileData) =>
            JsonConvert.DeserializeObject<IEnumerable<FileTypeInfo>>(flatFileData);

        /// <summary>
        /// Determines whether the specified input is text.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="hasBOM">if set to <c>true</c> [has bom].</param>
        /// <returns><c>true</c> if the specified input is text; otherwise, <c>false</c>.</returns>
        private bool IsText(byte[] input, out bool hasBOM)
        {
            hasBOM = false;

            bool isAscii = IsAscii(input);

            return isAscii || IsUTF8(input, out hasBOM);
        }

        /// <summary>
        /// Determines whether the specified input is ASCII.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns><c>true</c> if the specified input is ASCII; otherwise, <c>false</c>.</returns>
        private bool IsAscii(byte[] input)
        {
            const byte maxAscii = 0x7F;
            foreach (var b in input)
            {
                if (b > maxAscii)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether [is ut f8] [the specified input].
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="hasBOM">if set to <c>true</c> [has bom].</param>
        /// <returns><c>true</c> if [is ut f8] [the specified input]; otherwise, <c>false</c>.</returns>
        private bool IsUTF8(byte[] input, out bool hasBOM)
        {
            var utf8WithBOM = new UTF8Encoding(true, true);
            bool isUTF8 = true;
            byte[] bom = utf8WithBOM.GetPreamble();
            int bomLength = bom.Length;

            hasBOM = false;

            if (input.Length >= bomLength && bom.SequenceEqual(input.Take(bomLength)))
            {
                try
                {
                    utf8WithBOM.GetString(input, bomLength, input.Length - bomLength);
                    hasBOM = true;
                }
                catch (ArgumentException)
                {
                    // not utf8 due to exception
                    isUTF8 = false;
                }
            }

            if (isUTF8 && !hasBOM)
            {
                var utf8WithoutBOM = new UTF8Encoding(false, true);
                try
                {
                    utf8WithoutBOM.GetString(input, 0, input.Length);
                    isUTF8 = true;
                }
                catch (ArgumentException)
                {
                    isUTF8 = false;
                }
            }

            return isUTF8;
        }
    }
}