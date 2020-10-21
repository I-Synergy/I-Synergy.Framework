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
        /// <param name="_self">The self.</param>
        /// <returns>MemoryStream.</returns>
        public static MemoryStream ToMemoryStream(this byte[] _self)
        {
            MemoryStream result = null;

            if (_self != null && _self.Length > 0)
            {
                result = new MemoryStream();
                result.Write(_self, 0, _self.Length);
                result.Position = 0;
            }

            return result;
        }
    }
}
