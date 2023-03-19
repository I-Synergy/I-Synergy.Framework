using System.Runtime.InteropServices;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class ByteExtensions.
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        /// Converts the byte array2 stream.
        /// </summary>
        /// <param name="_self">The self.</param>
        /// <returns>MemoryStream.</returns>
        public static MemoryStream ToMemoryStream(this byte[] _self)
        {
            MemoryStream result = null;

            if (_self is not null && _self.Length > 0)
            {
                result = new MemoryStream();
                result.Write(_self, 0, _self.Length);
                result.Position = 0;
            }

            return result;
        }

        /// <summary>
        ///   Deserializes (converts) a byte array to a given structure type.
        /// </summary>
        /// 
        /// <remarks>
        ///  This is a potentiality unsafe operation.
        /// </remarks>
        /// 
        /// <param name="rawData">The byte array containing the serialized object.</param>
        /// <param name="position">The starting position in the rawData array where the object is located.</param>
        /// <returns>The object stored in the byte array.</returns>
        /// 
        public static T ToStruct<T>(this byte[] rawData, int position = 0)
            where T : struct
        {
            Type type = typeof(T);
            int rawsize = Marshal.SizeOf(type);

            if (rawsize > (rawData.Length - position))
                throw new ArgumentException("The given array is smaller than the object size.");

            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawData, position, buffer, rawsize);
            T obj = (T)Marshal.PtrToStructure(buffer, type);
            Marshal.FreeHGlobal(buffer);
            return obj;
        }
    }
}
