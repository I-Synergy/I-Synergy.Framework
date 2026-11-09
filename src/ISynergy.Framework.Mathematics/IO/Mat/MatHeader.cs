namespace ISynergy.Framework.Mathematics.IO.Mat;

using System.Runtime.InteropServices;



/// <summary>
///   Header of a MAT-file, version 5.
/// </summary>
/// <remarks>
///   The header is 128 bytes long: a descriptive text field of 116 bytes,
///   followed by the subsystem data offset (8 bytes), the format version
///   (2 bytes) and the endian indicator (2 bytes).
/// </remarks>
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
