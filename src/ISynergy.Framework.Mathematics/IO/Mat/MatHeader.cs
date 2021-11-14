namespace ISynergy.Framework.Mathematics.IO.Mat
{
    /*
            0   ||   1   ||   2   ||   3   ||   4   ||   5   ||   6   ||   7    
        +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ||                                                               ||   8
        ||                                                               ||  16
        ||                                                               ||  24
        ||                                                               ||  32
        ||                                                               ||  40 
        ||                                                               ||  48
        ||                                                               ||  56
        ||                         Descriptive text                      ||  64
        ||                            (116 bytes)                        ||  72     
        ||                                                               ||  80
        ||                                                               ||  88
        ||                                                               ||  96
        ||                                                               || 104
        ||                                                               || 112
        ||                               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|
        ||                               ||       subsys data offset      || 120
        +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ||       subsys data offset      ||  version    ||   endian ind.   |
        +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                     MAT-file header format (v5) - 128 bytes
         
        */
    [StructLayout(LayoutKind.Sequential, Size = 128)]
    internal struct MatHeader
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 124)]
        public string TextField;

        public int SubsystemDataOffset;

        public short Version;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string Endian;
    }
}
