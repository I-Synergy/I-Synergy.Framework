using System.Runtime.InteropServices;

namespace ISynergy.Framework.Clipboard.Wpf
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct BITMAPFILEHEADER
    {
        public static readonly short BM = 0x4d42;

        // BM
        public short bfType;

        public int bfSize;
        public short bfReserved1;
        public short bfReserved2;
        public int bfOffBits;
    }
}
