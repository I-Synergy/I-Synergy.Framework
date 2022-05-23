using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Binary reader extensions.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
        public static bool Read<T>(this BinaryReader stream, out T structure)
            where T : struct
        {
            var type = typeof(T);

            int size = Marshal.SizeOf(type);
            byte[] buffer = new byte[size];
            if (stream.Read(buffer, 0, buffer.Length) == 0)
            {
                structure = default(T);
                return false;
            }

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return true;
        }

        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
        public static T[][] ReadJagged<T>(this BinaryReader stream, int rows, int columns)
            where T : struct
        {
            var type = typeof(T);
            int size = Marshal.SizeOf(type);
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
        public static T[,] ReadMatrix<T>(this BinaryReader stream, int rows, int columns)
            where T : struct
        {
            return (T[,])ReadMatrix(stream, typeof(T), rows, columns);
        }

        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
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
}
