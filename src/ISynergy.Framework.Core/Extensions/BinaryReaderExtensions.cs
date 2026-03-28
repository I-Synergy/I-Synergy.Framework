using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Binary reader extensions.
/// </summary>
public static class BinaryReaderExtensions
{
    /// <summary>
    ///   Reads a <c>struct</c> from a stream.
    /// </summary>
    /// 
    public static bool Read<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>(this BinaryReader stream, out T structure)
        where T : struct
    {
        int size = Marshal.SizeOf<T>();
        byte[] buffer = new byte[size];
        if (stream.Read(buffer, 0, buffer.Length) == 0)
        {
            structure = default(T);
            return false;
        }

        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        try
        {
            structure = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
        }
        finally
        {
            handle.Free();
        }

        return true;
    }

    /// <summary>
    ///   Reads a <c>struct</c> from a stream.
    /// </summary>
    /// 
    public static T[][] ReadJagged<T>(this BinaryReader stream, int rows, int columns)
        where T : struct
    {
        int size = Marshal.SizeOf<T>();
        var buffer = new byte[size * columns];
        T[][] matrix = new T[rows][];
        for (int i = 0; i < matrix.Length; i++)
            matrix[i] = new T[columns];

        for (int i = 0; i < matrix.Length; i++)
        {
            stream.Read(buffer, 0, buffer.Length);
            Buffer.BlockCopy(buffer, 0, matrix[i], 0, buffer.Length);
        }

        return matrix;
    }

    /// <summary>
    ///   Reads a <c>struct</c> from a stream.
    /// </summary>
    ///
    [RequiresDynamicCode("Calls the non-generic ReadMatrix overload which requires dynamic code generation.")]
    public static T[,] ReadMatrix<T>(this BinaryReader stream, int rows, int columns)
        where T : struct
    {
        return (T[,])ReadMatrix(stream, typeof(T), rows, columns);
    }

    /// <summary>
    ///   Reads a <c>struct</c> from a stream.
    /// </summary>
    /// 
    [RequiresDynamicCode("Marshal.SizeOf and Array.CreateInstance with runtime types require dynamic code generation.")]
    public static Array ReadMatrix(this BinaryReader stream, Type type, params int[] lengths)
    {
        int size = Marshal.SizeOf(type);
        int total = 1;
        for (int i = 0; i < lengths.Length; i++)
            total *= lengths[i];
        var buffer = new byte[size * total];
        var matrix = Array.CreateInstance(type, lengths);

        stream.Read(buffer, 0, buffer.Length);
        Buffer.BlockCopy(buffer, 0, matrix, 0, buffer.Length);

        return matrix;
    }
}
