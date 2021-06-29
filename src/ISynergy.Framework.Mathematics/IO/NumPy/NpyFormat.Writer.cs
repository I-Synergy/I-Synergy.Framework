using ISynergy.Framework.Mathematics;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ISynergy.Framework.Mathematics.IO
{
    public static partial class NpyFormat
    {
        /// <summary>
        ///     Saves the specified array to an array of bytes.
        /// </summary>
        /// <param name="array">The array to be saved to the array of bytes.</param>
        /// <returns>A byte array containig the saved array.</returns>
        public static byte[] Save(Array array)
        {
            using (var stream = new MemoryStream())
            {
                Save(array, stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        ///     Saves the specified array to the disk using the npy format.
        /// </summary>
        /// <param name="array">The array to be saved to disk.</param>
        /// <param name="path">The disk path under which the file will be saved.</param>
        /// <returns>The number of bytes written when saving the file to disk.</returns>
        public static ulong Save(Array array, string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                return Save(array, stream);
            }
        }

        /// <summary>
        ///     Saves the specified array to a stream using the npy format.
        /// </summary>
        /// <param name="array">The array to be saved to disk.</param>
        /// <param name="stream">The stream to which the file will be saved.</param>
        /// <returns>The number of bytes written when saving the file to disk.</returns>
        public static ulong Save(Array array, Stream stream)
        {
            using (var writer = new BinaryWriter(stream
#if !NET35 && !NET40
                , Encoding.ASCII, true
#endif
            ))
            {
                Type type;
                int maxLength;
                var dtype = GetDtypeFromType(array, out type, out maxLength);

                var shape = array.GetLength(max: true);

                var bytesWritten = (ulong)writeHeader(writer, dtype, shape);

                if (array.IsJagged())
                {
                    if (type == typeof(string))
                        return bytesWritten + writeStringMatrix(writer, array, maxLength, shape);
                    return bytesWritten + writeValueJagged(writer, array, maxLength, shape);
                }

                if (type == typeof(string))
                    return bytesWritten + writeStringMatrix(writer, array, maxLength, shape);
                return bytesWritten + writeValueMatrix(writer, array, maxLength, shape);
            }
        }

        private static ulong writeValueMatrix(BinaryWriter reader, Array matrix, int bytes, int[] shape)
        {
            var total = 1;
            for (var i = 0; i < shape.Length; i++)
                total *= shape[i];
            var buffer = new byte[bytes * total];

            Buffer.BlockCopy(matrix, 0, buffer, 0, buffer.Length);
            reader.Write(buffer, 0, buffer.Length);

#if NETSTANDARD1_4
            return (ulong)buffer.Length;
#else
            return (ulong)buffer.LongLength;
#endif
        }

        private static ulong writeValueJagged(BinaryWriter reader, Array matrix, int bytes, int[] shape)
        {
            var last = shape[shape.Length - 1];
            var buffer = new byte[bytes * last];
            var first = shape.Get(0, -1);

            ulong writtenBytes = 0;
            foreach (var arr in matrix.Enumerate<Array>(first))
            {
                Array.Clear(buffer, arr.Length, buffer.Length - buffer.Length);
                Buffer.BlockCopy(arr, 0, buffer, 0, buffer.Length);
                reader.Write(buffer, 0, buffer.Length);
#if NETSTANDARD1_4
                writtenBytes += (ulong)buffer.Length;
#else
                writtenBytes += (ulong)buffer.LongLength;
#endif
            }

            return writtenBytes;
        }

        private static ulong writeStringMatrix(BinaryWriter reader, Array matrix, int bytes, int[] shape)
        {
            var buffer = new byte[bytes];
            var empty = new byte[bytes];
            empty[0] = byte.MinValue;
            for (var i = 1; i < empty.Length; i++)
                empty[i] = byte.MaxValue;

            ulong writtenBytes = 0;

            unsafe
            {
                fixed (byte* b = buffer)
                {
                    foreach (var s in matrix.Enumerate<string>(shape))
                    {
                        if (s != null)
                        {
                            var c = 0;
                            for (var i = 0; i < s.Length; i++)
                                b[c++] = (byte)s[i];
                            for (; c < buffer.Length; c++)
                                b[c] = byte.MinValue;

                            reader.Write(buffer, 0, bytes);
                        }
                        else
                        {
                            reader.Write(empty, 0, bytes);
                        }

#if NETSTANDARD1_4
                        writtenBytes += (ulong)buffer.Length;
#else
                        writtenBytes += (ulong)buffer.LongLength;
#endif
                    }
                }
            }

            return writtenBytes;
        }


        private static int writeHeader(BinaryWriter writer, string dtype, int[] shape)
        {
            // The first 6 bytes are a magic string: exactly "x93NUMPY"

            char[] magic = { 'N', 'U', 'M', 'P', 'Y' };
            writer.Write((byte)147);
            writer.Write(magic);
            writer.Write((byte)1); // major
            writer.Write((byte)0); // minor;

            var tuple = string.Join(", ", shape.Select(i => i.ToString()).ToArray());
            var header = "{{'descr': '{0}', 'fortran_order': False, 'shape': ({1}), }}";
            header = string.Format(header, dtype, tuple);
            var preamble = 10; // magic string (6) + 4

            var len = header.Length + 1; // the 1 is to account for the missing \n at the end
            var headerSize = len + preamble;

            var pad = 16 - headerSize % 16;
            header = header.PadRight(header.Length + pad);
            header += "\n";
            headerSize = header.Length + preamble;

            if (headerSize % 16 != 0)
                throw new Exception();

            writer.Write((ushort)header.Length);
            for (var i = 0; i < header.Length; i++)
                writer.Write((byte)header[i]);

            return headerSize;
        }

        private static string GetDtypeFromType(Array array, out Type type, out int bytes)
        {
            type = array.GetInnerMostType();

            bytes = 1;

            if (type == typeof(string))
            {
                foreach (var s in array.Enumerate<string>())
                    if (s.Length > bytes)
                        bytes = s.Length;
            }
            else if (type == typeof(bool))
            {
                bytes = 1;
            }
            else
            {
#pragma warning disable 618 // SizeOf would be Obsolete
                bytes = Marshal.SizeOf(type);
#pragma warning restore 618 // SizeOf would be Obsolete
            }

            if (type == typeof(bool))
                return "|b1";
            if (type == typeof(byte))
                return "|i1";
            if (type == typeof(short))
                return "<i2";
            if (type == typeof(int))
                return "<i4";
            if (type == typeof(long))
                return "<i8";
            if (type == typeof(ushort))
                return "<u2";
            if (type == typeof(uint))
                return "<u4";
            if (type == typeof(ulong))
                return "<u8";
            if (type == typeof(float))
                return "<f4";
            if (type == typeof(double))
                return "<f8";
            if (type == typeof(string))
                return "|S" + bytes;

            throw new NotSupportedException();
        }
    }
}