using System.IO;

namespace ISynergy.Framework.Core.Extensions
{
    public static class ByteExtensions
    {
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
