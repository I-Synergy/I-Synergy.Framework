using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ISynergy.Framework.Mathematics.IO.Mat
{
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
    internal enum ArrayFlagsType : byte
    {
        None = 0,
        Logical = 2,
        Global = 4,
        Complex = 8
    }
}