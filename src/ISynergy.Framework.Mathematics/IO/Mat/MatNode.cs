using ISynergy.Framework.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
namespace ISynergy.Framework.Mathematics.IO
{
    /// <summary>
    ///     Node object contained in <see cref="MatReader">.MAT file</see>.
    ///     A node can contain a matrix object, a string, or another nodes.
    /// </summary>
    public class MatNode : IEnumerable<MatNode>
    {
        private readonly int bytes;

        private readonly bool compressed;

        private readonly int[] dimensions;
        private readonly int length;
        private readonly MatReader matReader;
        private readonly long matrixOffset;
        private readonly MatDataType matType;

        private readonly int readBytes;

        private readonly BinaryReader reader;
        private readonly long startOffset;
        private readonly Type type;
        private readonly int typeSize;
        private object value;
        internal unsafe MatNode(MatReader matReader, BinaryReader reader, long offset, MatDataTag tag, bool lazy)
        {
            // TODO: Completely refactor this method.
            this.matReader = matReader;

            // int originalBytes = tag.NumberOfBytes;
            Fields = new Dictionary<string, MatNode>();

            startOffset = offset;
            this.reader = reader;

            if (tag.DataType == MatDataType.miCOMPRESSED)
            {
                compressed = true;

                // Read zlib's streams with Deflate using a little trick
                // http://george.chiramattel.com/blog/2007/09/deflatestream-block-length-does-not-match.html

                reader.ReadBytes(2); // ignore zlib headers

                reader = new BinaryReader(new DeflateStream(reader.BaseStream,
                    CompressionMode.Decompress, true));

                readBytes += 8;
                if (!reader.Read(out tag))
                    throw new NotSupportedException("Invalid reader at position " + readBytes + ".");
            }

            if (tag.DataType != MatDataType.miMATRIX)
                throw new NotSupportedException("Unexpected data type at position " + readBytes + ".");

            readBytes += 8;
            MatDataTag flagsTag;
            if (!reader.Read(out flagsTag))
                throw new NotSupportedException("Invalid flags tag at position " + readBytes + ".");

            if (flagsTag.DataType != MatDataType.miUINT32)
                throw new NotSupportedException("Unexpected flags data type at position " + readBytes + ".");

            readBytes += 8;

            ArrayFlags flagsElement;
            if (!reader.Read(out flagsElement))
                throw new NotSupportedException("Invalid flags element at position " + readBytes + ".");

            if (flagsElement.Class == MatArrayType.mxOBJECT_CLASS)
                throw new NotSupportedException("Unexpected object class flag at position " + readBytes + ".");
            readBytes += 8;
            MatDataTag dimensionsTag;
            if (!reader.Read(out dimensionsTag))
                throw new NotSupportedException("Invalid dimensions tag at position " + readBytes + ".");

            if (dimensionsTag.DataType != MatDataType.miINT32)
                throw new NotSupportedException("Invalid dimensions data type at position " + readBytes + ".");

            var numberOfDimensions = dimensionsTag.NumberOfBytes / 4;
            dimensions = new int[numberOfDimensions];
            for (var i = dimensions.Length - 1; i >= 0; i--)
                dimensions[i] = reader.ReadInt32();

            readBytes += dimensions.Length * 4;

            readBytes += 8;
            MatDataTag nameTag;
            if (!reader.Read(out nameTag))
                throw new NotSupportedException("Invalid name tag at position " + readBytes + ".");

            if (nameTag.DataType != MatDataType.miINT8)
                throw new NotSupportedException("Invalid name data type at position " + readBytes + ".");

            if (nameTag.IsSmallFormat)
            {
                Name = new string((sbyte*)nameTag.SmallData_Value, 0, nameTag.SmallData_NumberOfBytes);
            }
            else
            {
                readBytes += nameTag.NumberOfBytes;
                Name = new string(reader.ReadChars(nameTag.NumberOfBytes));
                align(reader, nameTag.NumberOfBytes);
            }

            Name = Name.Trim();

            if (flagsElement.Class == MatArrayType.mxSPARSE_CLASS)
            {
                readBytes += 8;
                MatDataTag irTag;
                if (!reader.Read(out irTag))
                    throw new NotSupportedException("Invalid sparse row tag at position " + readBytes + ".");

                // read ir
                var ir = new int[irTag.NumberOfBytes / 4];
                for (var i = 0; i < ir.Length; i++)
                    ir[i] = reader.ReadInt32();
                align(reader, irTag.NumberOfBytes);

                readBytes += 8;
                MatDataTag icTag;
                if (!reader.Read(out icTag))
                    throw new NotSupportedException("Invalid sparse column tag at position " + readBytes + ".");

                // read ic
                var ic = new int[icTag.NumberOfBytes / 4];
                for (var i = 0; i < ic.Length; i++)
                    ic[i] = reader.ReadInt32();
                align(reader, icTag.NumberOfBytes);
                // read values
                readBytes += 8;
                MatDataTag valuesTag;
                if (!reader.Read(out valuesTag))
                    throw new NotSupportedException("Invalid values tag at position " + readBytes + ".");

                var matType = valuesTag.DataType;
                type = MatReader.Translate(matType);
                typeSize = Marshal.SizeOf(type);
                length = valuesTag.NumberOfBytes / typeSize;
                bytes = valuesTag.NumberOfBytes;

                var rawData = reader.ReadBytes(bytes);
                align(reader, rawData.Length);

                if (matType == MatDataType.miINT64 || matType == MatDataType.miUINT64)
                    for (var i = 7; i < rawData.Length; i += 8)
                    {
                        var b = rawData[i];
                        var bit = (b & (1 << 6)) != 0;
                        if (bit)
                            rawData[i] |= 1 << 7;
                        else
                            rawData[i] = (byte)(b & ~(1 << 7));
                    }

                var array = Array.CreateInstance(type, length);
                Buffer.BlockCopy(rawData, 0, array, 0, rawData.Length);
                value = new MatSparse(ir, ic, array);
            }
            else if (flagsElement.Class == MatArrayType.mxCELL_CLASS)
            {
                var readBytes2 = 0;
                var toRead = tag.NumberOfBytes - readBytes;
                var cellI = 0;

                while (readBytes2 < toRead)
                {
                    // Read first MAT data element
                    MatDataTag elementTag;
                    if (!reader.Read(out elementTag))
                        throw new NotSupportedException("Invalid element tag at position " + readBytes + ".");

                    // Create a new node from the current position
                    var node = new MatNode(matReader, reader, offset, elementTag, false);

                    node.Name = (cellI++).ToString();

                    Fields.Add(node.Name, node);

                    readBytes2 += elementTag.NumberOfBytes + 8;
                }
            }
            else if (flagsElement.Class == MatArrayType.mxSTRUCT_CLASS)
            {
                MatDataTag fieldNameLengthTag;
                if (!reader.Read(out fieldNameLengthTag))
                    throw new NotSupportedException("Invalid struct field name length tag at position " + readBytes +
                                                    ".");

                if (!fieldNameLengthTag.IsSmallFormat)
                    throw new NotSupportedException(
                        "Small format struct field name length is not supported at position " + readBytes + ".");

                var fieldNameLength = *(int*)fieldNameLengthTag.SmallData_Value;

                if (fieldNameLengthTag.DataType != MatDataType.miINT32)
                    throw new NotSupportedException("Unexpected struct field name length data type at position " +
                                                    readBytes + ".");

                MatDataTag fieldNameTag;
                if (!reader.Read(out fieldNameTag))
                    throw new NotSupportedException("Invalid struct field name at position " + readBytes + ".");

                if (fieldNameTag.DataType != MatDataType.miINT8)
                    throw new NotSupportedException("Unexpected struct field name data type at position " + readBytes +
                                                    ".");

                var fields = fieldNameTag.NumberOfBytes / fieldNameLength;
                var names = new string[fields];
                for (var i = 0; i < names.Length; i++)
                {
                    var charNames = reader.ReadChars(fieldNameLength);
                    var terminator = Array.IndexOf(charNames, '\0');
                    names[i] = new string(charNames, 0, terminator);
                }

                align(reader, fieldNameTag.NumberOfBytes);

                for (var i = 0; i < names.Length; i++)
                {
                    Debug.WriteLine("reading " + names[i]);

                    // Read first MAT data element
                    MatDataTag elementTag;
                    if (!reader.Read(out elementTag))
                        throw new NotSupportedException("Invalid struct element at position " + readBytes + ".");

                    if (elementTag.DataType == MatDataType.miINT32)
                        throw new NotSupportedException("Unexpected struct element data type at position " + readBytes +
                                                        ".");

                    // Create a new node from the current position
                    var node = new MatNode(matReader, reader, offset, elementTag, false);

                    node.Name = names[i];

                    Fields.Add(node.Name, node);
                }
            }
            else
            {
                readBytes += 8;
                MatDataTag contentsTag;
                if (!reader.Read(out contentsTag))
                    throw new NotSupportedException("Invalid contents tag at position " + readBytes + ".");

                if (contentsTag.IsSmallFormat)
                {
                    matType = contentsTag.SmallData_Type;
                    if (matType == MatDataType.miUTF8)
                    {
                        value = new string((sbyte*)contentsTag.SmallData_Value, 0,
                            contentsTag.SmallData_NumberOfBytes);
                    }
                    else
                    {
                        type = MatReader.Translate(matType);
                        typeSize = Marshal.SizeOf(type);
                        length = 1;
                        for (var i = 0; i < dimensions.Length; i++)
                            length *= dimensions[i];
                        var array = Array.CreateInstance(type, dimensions);
                        var rawData = new byte[4];
                        for (var i = 0; i < rawData.Length; i++)
                            rawData[i] = contentsTag.SmallData_Value[i];
                        Buffer.BlockCopy(rawData, 0, array, 0, length);

                        if (matReader.Transpose)
                            array = array.Transpose();

                        value = array;
                    }
                }
                else
                {
                    matType = contentsTag.DataType;
                    if (matType == MatDataType.miMATRIX)
                    {
                        // Create a new node from the current position
                        value = new MatNode(matReader, reader, offset, contentsTag, false);
                    }
                    else if (matType == MatDataType.miUTF8)
                    {
                        var utf8 = reader.ReadChars(contentsTag.NumberOfBytes);
                        value = new string(utf8);
                        align(reader, utf8.Length);
                    }
                    else
                    {
                        type = MatReader.Translate(matType);
                        typeSize = Marshal.SizeOf(type);
                        length = contentsTag.NumberOfBytes / typeSize;
                        bytes = contentsTag.NumberOfBytes;

                        if (!lazy)
                            value = read(reader);
                    }
                }
            }

            if (!compressed && lazy)
                matrixOffset = reader.BaseStream.Position;
        }

        /// <summary>
        ///     Gets the name of this node.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the child nodes contained at this node.
        /// </summary>
        public Dictionary<string, MatNode> Fields { get; }

        /// <summary>
        ///     Gets the object value contained at this node, if any.
        ///     Its type can be known by checking the <see cref="Type" />
        ///     property of this node.
        /// </summary>
        public object Value
        {
            get
            {
                if (value == null && type != null)
                    value = read();
                return value;
            }
        }

        /// <summary>
        ///     Gets the type of the object value contained in this node.
        /// </summary>
        public Type ValueType => Value.GetType();

        /// <summary>
        ///     Gets the number of child objects contained in this node.
        /// </summary>
        public int Count => Fields.Count;

        /// <summary>
        ///     Gets the child fields contained under the given name.
        /// </summary>
        /// <param name="name">The name of the field to be retrieved.</param>
        public MatNode this[string name] => Fields[name];

        /// <summary>
        ///     Gets the child fields contained under the given name.
        /// </summary>
        /// <param name="name">The name of the field to be retrieved.</param>
        public MatNode this[int name] => Fields[name.ToString()];
        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<MatNode> GetEnumerator()
        {
            return Fields.Values.GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Fields.Values.GetEnumerator();
        }

        /// <summary>
        ///     Gets the object value contained at this node, if any.
        ///     Its type can be known by checking the <see cref="Type" />
        ///     property of this node.
        /// </summary>
        /// <typeparam name="T">The object type, if known.</typeparam>
        /// <returns>The object stored at this node.</returns>
        public T GetValue<T>()
        {
            if (Value is T)
                return (T)Value;

            if (typeof(T).IsArray)
            {
                var targetType = typeof(T).GetElementType();
                var src = Value as Array;
                var dst = Array.CreateInstance(targetType, dimensions);

                if (matReader.Transpose)
                    dst = dst.Transpose();

                foreach (var idx in src.GetIndices())
                    dst.SetValue(Convert.ChangeType(src.GetValue(idx), targetType), idx);

                return (T)Convert.ChangeType(dst, typeof(T));
            }

            throw new InvalidCastException();
        }

        private static void align(BinaryReader reader, int rreadBytes)
        {
            var mod = rreadBytes % 8;
            if (mod != 0) // need to be 8 bytes aligned
                reader.ReadBytes(8 - mod);
        }

        private object read()
        {
            var reader = this.reader;

            if (compressed)
            {
                reader.BaseStream.Seek(startOffset + 8 + 2, SeekOrigin.Begin);
                reader = new BinaryReader(new DeflateStream(reader.BaseStream,
                    CompressionMode.Decompress, true));
                reader.ReadBytes(readBytes);
            }
            else
            {
                reader.BaseStream.Seek(matrixOffset, SeekOrigin.Begin);
            }

            var array = read(reader);

            return array;
        }

        private Array read(BinaryReader reader)
        {
            var rawData = reader.ReadBytes(bytes);
            align(reader, rawData.Length);

            if (matType == MatDataType.miINT64 || matType == MatDataType.miUINT64)
                for (var i = 7; i < rawData.Length; i += 8)
                {
                    var b = rawData[i];
                    var bit = (b & (1 << 6)) != 0;
                    if (bit)
                        rawData[i] |= 1 << 7;
                    else
                        rawData[i] = (byte)(b & ~(1 << 7));
                }

            var array = Array.CreateInstance(type, dimensions);
            Buffer.BlockCopy(rawData, 0, array, 0, rawData.Length);
            if (matReader.Transpose)
                array = array.Transpose();
            return array;
        }
    }
}