namespace ISynergy.Framework.Mathematics.IO.Mat;

using System.Runtime.InteropServices;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


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
