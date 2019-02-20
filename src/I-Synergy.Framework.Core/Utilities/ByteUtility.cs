using System.Text;

namespace ISynergy.Utilities
{
    public static class ByteUtility
    {
        public static string WriteBytesToString(string Input, byte[] bytes, int start)
        {
            StringBuilder g = new StringBuilder(Input);
            string temp;
            int ByteNum = 0;
            int CharPos = start;
            int NumBytes = (int)bytes.LongLength;
            for (int i = 0; i < NumBytes; i++)
            {
                temp = string.Format("{0:x2}", bytes[ByteNum++]);
                g[CharPos++] = (temp.ToCharArray())[0];
                g[CharPos++] = (temp.ToCharArray())[1];
            }
            return g.ToString();
        }
    }
}
