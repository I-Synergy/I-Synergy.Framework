using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Binary writer extensions.
    /// </summary>
    public static class BinaryWriterExtensions
    {
        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
        public static bool Write<T>(this BinaryWriter stream, T[] array)
            where T : struct
        {
            var type = typeof(T);
            int size = Marshal.SizeOf(type);
            byte[] buffer = new byte[size * array.Length];

            Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
            stream.Write(buffer, 0, buffer.Length);

            return true;
        }

        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
        public static bool Write<T>(this BinaryWriter stream, T[][] array)
            where T : struct
        {
            var type = typeof(T);
            int size = Marshal.SizeOf(type);
            byte[] buffer = new byte[size * array[0].Length];

            for (int i = 0; i < array.Length; i++)
            {
                Buffer.BlockCopy(array[i], 0, buffer, 0, buffer.Length);
                stream.Write(buffer, 0, buffer.Length);
            }

            return true;
        }

        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
        public static bool Write<T>(this BinaryWriter stream, T[,] array)
            where T : struct
        {
            var type = typeof(T);
            int size = Marshal.SizeOf(type);
            byte[] buffer = new byte[size * array.Length];

            Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
            stream.Write(buffer, 0, buffer.Length);

            return true;
        }
    }
}
