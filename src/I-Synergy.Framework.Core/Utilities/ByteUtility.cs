using System.Text;

namespace ISynergy.Framework.Core.Utilities
{
    public static class ByteUtility
    {
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
