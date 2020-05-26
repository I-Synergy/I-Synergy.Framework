using System.IO;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class ByteExtensions.
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        /// Converts the byte array2 stream.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>MemoryStream.</returns>
        public static MemoryStream ConvertByteArray2Stream(this byte[] self)
        {
            MemoryStream result = null;

            if (self != null)
            {
                result = new MemoryStream();
                result.Write(self, 0, self.Length);
                result.Position = 0;
            }

            return result;
        }
    }
}
