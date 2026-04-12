using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.IO.Mat;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
internal struct ArrayFlags
{
    [FieldOffset(0)] public MatArrayType Class;

    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
    [FieldOffset(1)]
    public ArrayFlagsType Flags;

    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
    [FieldOffset(4)]
    public int NonZeroElements;
}

[Flags]
internal enum ArrayFlagsType : byte // NOSONAR
{
    None = 0,
    Logical = 2,
    Global = 4,
    Complex = 8
}