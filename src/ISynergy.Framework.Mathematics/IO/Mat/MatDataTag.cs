namespace ISynergy.Framework.Mathematics.IO.Mat;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;



/*
  MAT-file v5 binary layout reference (documentation only).

  Regular data element: an eight byte header holding the data type in the
  first four bytes and the number of bytes of the payload in the next four
  bytes, followed by the variable sized payload.

  Small data element: an eight byte header holding the number of bytes in
  the first four bytes and the data type in the next four bytes, followed by
  a fixed four byte payload packed into the remaining bytes of the header.
*/

/// <summary>
///   Four-byte buffer holding a small data element payload.
///   Replaces the previous fixed-size buffer so the containing struct no longer needs unsafe code.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
internal struct SmallDataBuffer
{
    private byte _element0;
    private byte _element1;
    private byte _element2;
    private byte _element3;

    /// <summary>
    ///   Gets or sets the byte at the given position.
    /// </summary>
    public byte this[int index]
    {
        get => index switch
        {
            0 => _element0,
            1 => _element1,
            2 => _element2,
            3 => _element3,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
        set
        {
            switch (index)
            {
                case 0: _element0 = value; break;
                case 1: _element1 = value; break;
                case 2: _element2 = value; break;
                case 3: _element3 = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }

    /// <summary>
    ///   Returns a read-only view over the four bytes of the buffer.
    /// </summary>
    public ReadOnlySpan<byte> AsSpan() => MemoryMarshal.CreateReadOnlySpan(ref _element0, 4);
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
internal struct MatDataTag
{
    [FieldOffset(0)]
    public MatDataType DataType;

    [FieldOffset(4)]
    public int NumberOfBytes;

    [FieldOffset(2)]
    public short SmallData_NumberOfBytes;

    [FieldOffset(0)]
    public MatDataType SmallData_Type;

    [FieldOffset(4)]
    public SmallDataBuffer SmallData_Value;

    public bool IsSmallFormat { get { return SmallData_NumberOfBytes != 0; } }

}
