using ISynergy.Framework.Mathematics.Matrices;
using System.Collections;
using System.Text;

namespace ISynergy.Framework.Mathematics.IO.NumPy;

#if !NET35 && !NET40
/// <summary>
///     Provides static methods to save and load files saved in NumPy's .npy format.
/// </summary>
/// <seealso cref="NpzFormat" />
#else
/// <summary>
///   Provides static methods to save and load files saved in NumPy's .npy format. 
/// </summary>
/// 
#endif
public static partial class NpyFormat
{
    /// <summary>
    ///     Loads an array of the specified type from a byte array.
    /// </summary>
    /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
    /// <param name="bytes">The bytes that contain the matrix to be loaded.</param>
    /// <returns>The array to be returned.</returns>
    public static T Load<T>(byte[] bytes)
        where T : class,
#if !NETSTANDARD1_4
        ICloneable,
#endif
        IList, ICollection, IEnumerable
#if !NET35
        , IStructuralComparable, IStructuralEquatable
#endif
    {
        if (typeof(T).IsJagged())
            return LoadJagged(bytes).To<T>();
        return LoadMatrix(bytes).To<T>();
    }

    /// <summary>
    ///     Loads an array of the specified type from a file in the disk.
    /// </summary>
    /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
    /// <param name="bytes">The bytes that contain the matrix to be loaded.</param>
    /// <param name="value">
    ///     The object to be read. This parameter can be used to avoid the
    ///     need of specifying a generic argument to this function.
    /// </param>
    /// <returns>The array to be returned.</returns>
    public static T Load<T>(byte[] bytes, out T value)
        where T : class,
#if !NETSTANDARD1_4
        ICloneable,
#endif
        IList, ICollection, IEnumerable
#if !NET35
        , IStructuralComparable, IStructuralEquatable
#endif
    {
        return value = Load<T>(bytes);
    }

    /// <summary>
    ///     Loads an array of the specified type from a file in the disk.
    /// </summary>
    /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
    /// <param name="path">The path to the file containing the matrix to be loaded.</param>
    /// <param name="value">
    ///     The object to be read. This parameter can be used to avoid the
    ///     need of specifying a generic argument to this function.
    /// </param>
    /// <returns>The array to be returned.</returns>
    public static T Load<T>(string path, out T value)
        where T : class,
#if !NETSTANDARD1_4
        ICloneable,
#endif
        IList, ICollection, IEnumerable
#if !NET35
        , IStructuralComparable, IStructuralEquatable
#endif
    {
        return value = Load<T>(path);
    }

    /// <summary>
    ///     Loads an array of the specified type from a stream.
    /// </summary>
    /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
    /// <param name="stream">The stream containing the matrix to be loaded.</param>
    /// <param name="value">
    ///     The object to be read. This parameter can be used to avoid the
    ///     need of specifying a generic argument to this function.
    /// </param>
    /// <returns>The array to be returned.</returns>
    public static T Load<T>(Stream stream, out T value)
        where T : class,
#if !NETSTANDARD1_4
        ICloneable,
#endif
        IList, ICollection, IEnumerable
#if !NET35
        , IStructuralComparable, IStructuralEquatable
#endif
    {
        return value = Load<T>(stream);
    }

    /// <summary>
    ///     Loads an array of the specified type from a file in the disk.
    /// </summary>
    /// <param name="path">The path to the file containing the matrix to be loaded.</param>
    /// <returns>The array to be returned.</returns>
    public static T Load<T>(string path)
        where T : class,
#if !NETSTANDARD1_4
        ICloneable,
#endif
        IList, ICollection, IEnumerable
#if !NET35
        , IStructuralComparable, IStructuralEquatable
#endif
    {
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return Load<T>(stream);
        }
    }

    /// <summary>
    ///     Loads an array of the specified type from a stream.
    /// </summary>
    /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
    /// <param name="stream">The stream containing the matrix to be loaded.</param>
    /// <returns>The array to be returned.</returns>
    public static T Load<T>(Stream stream)
        where T : class,
#if !NETSTANDARD1_4
        ICloneable,
#endif
        IList, ICollection, IEnumerable
#if !NET35
        , IStructuralComparable, IStructuralEquatable
#endif
    {
        if (typeof(T).IsJagged())
            return LoadJagged(stream).To<T>();
        return LoadMatrix(stream).To<T>();
    }
    /// <summary>
    ///     Loads a multi-dimensional array from an array of bytes.
    /// </summary>
    /// <param name="bytes">The bytes that contain the matrix to be loaded.</param>
    /// <returns>A multi-dimensional array containing the values available in the given stream.</returns>
    public static Array LoadMatrix(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        {
            return LoadMatrix(stream);
        }
    }

    /// <summary>
    ///     Loads a multi-dimensional array from a file in the disk.
    /// </summary>
    /// <param name="path">The path to the file containing the matrix to be loaded.</param>
    /// <returns>A multi-dimensional array containing the values available in the given stream.</returns>
    public static Array LoadMatrix(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return LoadMatrix(stream);
        }
    }

    /// <summary>
    ///     Loads a jagged array from an array of bytes.
    /// </summary>
    /// <param name="bytes">The bytes that contain the matrix to be loaded.</param>
    /// <returns>A jagged array containing the values available in the given stream.</returns>
    public static Array LoadJagged(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        {
            return LoadJagged(stream);
        }
    }

    /// <summary>
    ///     Loads a jagged array from a file in the disk.
    /// </summary>
    /// <param name="path">The path to the file containing the matrix to be loaded.</param>
    /// <returns>A jagged array containing the values available in the given stream.</returns>
    public static Array LoadJagged(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return LoadJagged(stream);
        }
    }
    /// <summary>
    ///     Loads a multi-dimensional array from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the matrix to be loaded.</param>
    /// <returns>A multi-dimensional array containing the values available in the given stream.</returns>
    public static Array LoadMatrix(Stream stream)
    {
        using (var reader = new BinaryReader(stream, Encoding.ASCII
#if !NET35 && !NET40
            , true
#endif
        ))
        {
            int bytes;
            Type type;
            int[] shape;
            if (!parseReader(reader, out bytes, out type, out shape))
                throw new FormatException();

            Array matrix = Matrix.Zeros(type, shape);

            if (type == typeof(string))
                return readStringMatrix(reader, matrix, bytes, type, shape);
            return readValueMatrix(reader, matrix, bytes, type, shape);
        }
    }

    /// <summary>
    ///     Loads a jagged array from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the matrix to be loaded.</param>
    /// <param name="trim">Pass true to remove null or empty elements from the loaded array.</param>
    /// <returns>A jagged array containing the values available in the given stream.</returns>
    public static Array LoadJagged(Stream stream, bool trim = true)
    {
        using (var reader = new BinaryReader(stream, Encoding.ASCII
#if !NET35 && !NET40
            , true
#endif
        ))
        {
            int bytes;
            Type type;
            int[] shape;
            if (!parseReader(reader, out bytes, out type, out shape))
                throw new FormatException();

            var matrix = Jagged.Zeros(type, shape);

            if (type == typeof(string))
            {
                var result = readStringMatrix(reader, matrix, bytes, type, shape);

                if (trim)
                    return result.Trim();
                return result;
            }

            return readValueJagged(reader, matrix, bytes, type, shape);
        }
    }
    private static Array readValueMatrix(BinaryReader reader, Array matrix, int bytes, Type type, int[] shape)
    {
        var total = 1;
        for (var i = 0; i < shape.Length; i++)
            total *= shape[i];
        var buffer = new byte[bytes * total];

        reader.Read(buffer, 0, buffer.Length);
        Buffer.BlockCopy(buffer, 0, matrix, 0, buffer.Length);

        return matrix;
    }

    private static Array readValueJagged(BinaryReader reader, Array matrix, int bytes, Type type, int[] shape)
    {
        var last = shape[shape.Length - 1];
        var buffer = new byte[bytes * last];

        var firsts = new int[shape.Length - 1];
        for (var i = 0; i < firsts.Length; i++)
            firsts[i] = -1;

        foreach (var p in matrix.GetIndices(true))
        {
            var changed = false;
            for (var i = 0; i < firsts.Length; i++)
                if (firsts[i] != p[i])
                {
                    firsts[i] = p[i];
                    changed = true;
                }

            if (!changed)
                continue;

            var arr = (Array)matrix.GetValue(true, firsts);

            reader.Read(buffer, 0, buffer.Length);
            Buffer.BlockCopy(buffer, 0, arr, 0, buffer.Length);
        }

        return matrix;
    }

    private static Array readStringMatrix(BinaryReader reader, Array matrix, int bytes, Type type, int[] shape)
    {
        var buffer = new byte[bytes];

        unsafe
        {
            fixed (byte* b = buffer)
            {
                foreach (var p in matrix.GetIndices(true))
                {
                    reader.Read(buffer, 0, bytes);
                    if (buffer[0] == byte.MinValue)
                    {
                        var isNull = true;
                        for (var i = 1; i < buffer.Length; i++)
                            if (buffer[i] != byte.MaxValue)
                            {
                                isNull = false;
                                break;
                            }

                        if (isNull)
                        {
                            matrix.SetValue(null, true, p);
                            continue;
                        }
                    }

#if NETSTANDARD1_4
                    String s = new String((char*)b);
#else
                    var s = new string((sbyte*)b);
#endif
                    matrix.SetValue(s, true, p);
                }
            }
        }

        return matrix;
    }
    private static bool parseReader(BinaryReader reader, out int bytes, out Type t, out int[] shape)
    {
        bytes = 0;
        t = null;
        shape = null;

        // The first 6 bytes are a magic string: exactly "x93NUMPY"
        if (reader.ReadChar() != 63) return false;
        if (reader.ReadChar() != 'N') return false;
        if (reader.ReadChar() != 'U') return false;
        if (reader.ReadChar() != 'M') return false;
        if (reader.ReadChar() != 'P') return false;
        if (reader.ReadChar() != 'Y') return false;

        var major = reader.ReadByte(); // 1
        var minor = reader.ReadByte(); // 0

        if (major != 1 || minor != 0)
            throw new NotSupportedException();

        var len = reader.ReadUInt16();

        var header = new string(reader.ReadChars(len));
        var mark = "'descr': '";
        var s = header.IndexOf(mark) + mark.Length;
        var e = header.IndexOf("'", s + 1);
        var type = header.Substring(s, e - s);
        bool? isLittleEndian;
        t = GetType(type, out bytes, out isLittleEndian);

        if (isLittleEndian.HasValue && isLittleEndian.Value == false)
            throw new Exception();

        mark = "'fortran_order': ";
        s = header.IndexOf(mark) + mark.Length;
        e = header.IndexOf(",", s + 1);
        var fortran = bool.Parse(header.Substring(s, e - s));

        if (fortran)
            throw new Exception();

        mark = "'shape': (";
        s = header.IndexOf(mark) + mark.Length;
        e = header.IndexOf(")", s + 1);
        shape = header.Substring(s, e - s).Split(',').Select(int.Parse).ToArray();

        return true;
    }

    private static Type GetType(string dtype, out int bytes, out bool? isLittleEndian)
    {
        isLittleEndian = IsLittleEndian(dtype);
        bytes = int.Parse(dtype.Substring(2));

        var typeCode = dtype.Substring(1);

        if (typeCode == "b1")
            return typeof(bool);
        if (typeCode == "i1")
            return typeof(sbyte);
        if (typeCode == "i2")
            return typeof(short);
        if (typeCode == "i4")
            return typeof(int);
        if (typeCode == "i8")
            return typeof(long);
        if (typeCode == "u1")
            return typeof(byte);
        if (typeCode == "u2")
            return typeof(ushort);
        if (typeCode == "u4")
            return typeof(uint);
        if (typeCode == "u8")
            return typeof(ulong);
        if (typeCode == "f4")
            return typeof(float);
        if (typeCode == "f8")
            return typeof(double);
        if (typeCode.StartsWith("S"))
            return typeof(string);

        throw new NotSupportedException();
    }

    private static bool? IsLittleEndian(string type)
    {
        bool? littleEndian = null;

        switch (type[0])
        {
            case '<':
                littleEndian = true;
                break;
            case '>':
                littleEndian = false;
                break;
            case '|':
                littleEndian = null;
                break;
            default:
                throw new Exception();
        }

        return littleEndian;
    }
}