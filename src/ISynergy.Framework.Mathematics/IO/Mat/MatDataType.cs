
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.IO.Mat;


internal enum MatDataType : short
{
    /// <summary>
    ///   8 bit, signed
    /// </summary>
    /// 
    miINT8 = 1,

    /// <summary>
    ///   8 bit, unsigned
    ///</summary>
    /// 
    miUINT8 = 2,

    /// <summary>
    ///   16-bit, signed
    /// </summary>
    /// 
    miINT16 = 3,

    /// <summary>
    ///   16-bit, unsigned
    /// </summary>
    /// 
    miUINT16 = 4,

    /// <summary>
    ///   32-bit, signed
    /// </summary>
    /// 
    miINT32 = 5,

    /// <summary>
    ///   32-bit, unsigned
    /// </summary>
    /// 
    miUINT32 = 6,

    /// <summary>
    ///   IEEE® 754 single format
    /// </summary>
    /// 
    miSINGLE = 7,

    /// <summary>
    ///   IEEE 754 double format
    /// </summary>
    /// 
    miDOUBLE = 9,

    /// <summary>
    ///   64-bit, signed
    /// </summary>
    /// 
    miINT64 = 12,

    /// <summary>
    ///   64-bit, unsigned
    /// </summary>
    /// 
    miUINT64 = 13,

    /// <summary>
    ///   MATLAB array
    /// </summary>
    /// 
    miMATRIX = 14,

    /// <summary>
    ///   Compressed Data
    /// </summary>
    /// 
    miCOMPRESSED = 15,

    /// <summary>
    ///   Unicode UTF-8 Encoded Character Data
    /// </summary>
    /// 
    miUTF8 = 16,

    /// <summary>
    ///   Unicode UTF-16 Encoded Character Data
    /// </summary>
    /// 
    miUTF16 = 17,

    /// <summary>
    ///   Unicode UTF-32 Encoded Character Data
    /// </summary>
    /// 
    miUTF32 = 18,
}
