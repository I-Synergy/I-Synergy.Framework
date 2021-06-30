using System.IO;
using System.Reflection;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Stream reader extensions.
    /// </summary>
    public static class StreamReaderExtensions
    {
        /// <summary>
        ///   Gets the underlying buffer position for a StreamReader.
        /// </summary>
        /// 
        /// <param name="reader">A StreamReader whose position will be retrieved.</param>
        /// 
        /// <returns>The current offset from the beginning of the 
        ///   file that the StreamReader is currently located into.</returns>
        /// 
        public static long GetPosition(this StreamReader reader)
        {
            // http://stackoverflow.com/a/17457085/262032
            var type = typeof(StreamReader).GetTypeInfo();
            char[] charBuffer = (char[])type.GetDeclaredField("_charBuffer").GetValue(reader);
            int charPos = (int)type.GetDeclaredField("_charPos").GetValue(reader);
            int byteLen = (int)type.GetDeclaredField("_byteLen").GetValue(reader);

            // The number of bytes that the already-read characters need when encoded.
            int numReadBytes = reader.CurrentEncoding.GetByteCount(charBuffer, 0, charPos);

            return reader.BaseStream.Position - byteLen + numReadBytes;
        }
    }
}
