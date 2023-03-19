using System.Text;

namespace ISynergy.Framework.Core.Utilities
{
    /// <summary>
    /// Class ByteUtility.
    /// </summary>
    public static class ByteUtility
    {
        /// <summary>
        /// Writes the bytes to string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">The start.</param>
        /// <returns>System.String.</returns>
        public static string WriteBytesToString(string input, byte[] bytes, int start)
        {
            var result = new StringBuilder(input);
            var byteNum = 0;
            var charPos = start;

            for (var i = 0; i < bytes.LongLength; i++)
            {
                var temp = string.Format("{0:x2}", bytes[byteNum++]);
                result[charPos++] = temp[0];
                result[charPos++] = temp[1];
            }

            return result.ToString();
        }
    }
}
