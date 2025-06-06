﻿using System.Runtime.InteropServices;

namespace ISynergy.Framework.UI.Converters;

public static class BinaryConverter
{
    public static T FromByteArray<T>(byte[] bytes) where T : struct
    {
        IntPtr ptr = IntPtr.Zero;

        try
        {
            int size = Marshal.SizeOf(typeof(T));
            ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);
            object? obj = Marshal.PtrToStructure(ptr, typeof(T));

            if (obj is null)
                throw new InvalidOperationException("Failed to convert byte array to structure.");

            return (T)obj;
        }
        finally
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }

    public static byte[] ToByteArray<T>(T obj) where T : struct
    {
        IntPtr ptr = IntPtr.Zero;

        try
        {
            int size = Marshal.SizeOf(typeof(T));
            ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            return bytes;
        }
        finally
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
