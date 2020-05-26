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
        /// <param name="Input">The input.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">The start.</param>
        /// <returns>System.String.</returns>
        public static string WriteBytesToString(string Input, byte[] bytes, int start)
        {
            var g = new StringBuilder(Input);
            string temp;
            var ByteNum = 0;
            var CharPos = start;
            var NumBytes = (int)bytes.LongLength;
            for (var i = 0; i < NumBytes; i++)
            {
                temp = string.Format("{0:x2}", bytes[ByteNum++]);
                g[CharPos++] = temp.ToCharArray()[0];
                g[CharPos++] = temp.ToCharArray()[1];
            }
            return g.ToString();
        }
    }
}
