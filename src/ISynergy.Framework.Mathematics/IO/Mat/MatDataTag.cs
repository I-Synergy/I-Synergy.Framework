namespace ISynergy.Framework.Mathematics.IO.Mat;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation
#pragma warning disable S125 // struct layout diagram comment is documentation, not commented code


/*
      
 0   ||   1   ||   2   ||   3   ||   4   ||   5   ||   6   ||   7    
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
||         data type          ||          number of bytes         |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
||                                                               |
~                       variable size                           ~
||                                                               |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

             MAT-file data element format (v5)
             8 bytes header / variable payload
 0   ||   1   ||   2   ||   3   ||   4   ||   5   ||   6   ||   7    
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
||   number of bytes  ||   data type   ||           data           |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

            MAT-file small data element format (v5)
                8 bytes header + fixed payload
     
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
