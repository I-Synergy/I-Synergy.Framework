using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Object extensions.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Perform a deep Copy of the object.
    /// </summary>
    /// <typeparam name="T">The type of object being copied.</typeparam>
    /// <param name="source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    public static T Clone<T>(this T source) =>
        JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(source), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    /// <summary>
    /// Checks if object is of a nullable type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self">The self.</param>
    /// <returns><c>true</c> if [is nullable type] [the specified self]; otherwise, <c>false</c>.</returns>
    public static bool IsNullableType<T>(this T self) where T : Type =>
        !self.IsValueType || Nullable.GetUnderlyingType(self) is not null;

    /// <summary>
    ///   Converts an object into another type, irrespective of whether
    ///   the conversion can be done at compile time or not. This can be
    ///   used to convert generic types to numeric types during runtime.
    /// </summary>
    /// 
    /// <typeparam name="T">The destination type.</typeparam>
    /// 
    /// <param name="value">The value to be converted.</param>
    /// 
    /// <returns>The result of the conversion.</returns>
    /// 
    public static T To<T>(this object value)
    {
        return (T)To(value, typeof(T));
    }

    /// <summary>
    ///   Converts an object into another type, irrespective of whether
    ///   the conversion can be done at compile time or not. This can be
    ///   used to convert generic types to numeric types during runtime.
    /// </summary>
    /// 
    /// <param name="value">The value to be converted.</param>
    /// <param name="type">The type that the value should be converted to.</param>
    /// 
    /// <returns>The result of the conversion.</returns>
    /// 
    public static object To(this object value, Type type)
    {
        if (value is null)
            return System.Convert.ChangeType(null, type);

        if (type.IsInstanceOfType(value))
            return value;

        if (type.GetTypeInfo().IsEnum)
            return Enum.ToObject(type, (int)System.Convert.ChangeType(value, typeof(int)));

        Type inputType = value.GetType();

        if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            MethodInfo setter = type.GetMethod("op_Implicit", [inputType]);
            return setter.Invoke(null, [value]);
        }

        var methods = new List<MethodInfo>();
        methods.AddRange(inputType.GetMethods(BindingFlags.Public | BindingFlags.Static));
        methods.AddRange(type.GetMethods(BindingFlags.Public | BindingFlags.Static));

        foreach (MethodInfo m in methods.EnsureNotNull())
        {
            if (m.IsPublic && m.IsStatic)
            {
                if ((m.Name == "op_Implicit" || m.Name == "op_Explicit") && m.ReturnType == type)
                {
                    ParameterInfo[] p = m.GetParameters();
                    if (p.Length == 1 && p[0].ParameterType.IsInstanceOfType(value))
                        return m.Invoke(null, [value]);
                }
            }
        }

        //if (value is IConvertible)
        //    return System.Convert.ChangeType(value, type);

        return System.Convert.ChangeType(value, type);
    }

    /// <summary>
    ///   Checks whether an object implements a method with the given name.
    /// </summary>
    /// 
    public static bool HasMethod(this object obj, string methodName)
    {
        try
        {
            return obj.GetType().GetMethod(methodName) is not null;
        }
        catch (AmbiguousMatchException)
        {
            // ambiguous means there is more than one result,
            // which means: a method with that name does exist
            return true;
        }
    }

    /// <summary>
    /// Determines whether the specified property name has property.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns><c>true</c> if the specified property name has property; otherwise, <c>false</c>.</returns>
    public static bool HasProperty(this object obj, string propertyName)
    {
        try
        {
            return obj.GetType().GetRuntimeProperty(propertyName) is not null;
        }
        catch (AmbiguousMatchException)
        {
            // ambiguous means there is more than one result,
            // which means: a property with that name does exist
            return true;
        }
    }

    /// <summary>
    ///   Determines whether <c>a > b</c>.
    /// </summary>
    /// 
    public static bool IsGreaterThan<T>(this T a, object b)
        where T : IComparable
    {
        return a.CompareTo(b) > 0;
    }

    /// <summary>
    ///   Determines whether <c>a >= b</c>.
    /// </summary>
    /// 
    public static bool IsGreaterThanOrEqual<T>(this T a, object b)
        where T : IComparable
    {
        return a.CompareTo(b) >= 0;
    }

    /// <summary>
    ///   Determines whether <c>a &lt; b</c>.
    /// </summary>
    /// 
    public static bool IsLessThan<T>(this T a, object b)
        where T : IComparable
    {
        return a.CompareTo(b) < 0;
    }

    /// <summary>
    ///   Determines whether <c>a &lt;= b</c>.
    /// </summary>
    /// 
    public static bool IsLessThanOrEqual<T>(this T a, object b)
        where T : IComparable
    {
        return a.CompareTo(b) <= 0;
    }

    /// <summary>
    ///  Serializes (converts) a structure to a byte array.
    /// </summary>
    /// 
    /// <param name="value">The structure to be serialized.</param>
    /// <returns>The byte array containing the serialized structure.</returns>
    /// 
    public static byte[] ToByteArray<T>(this T value)
        where T : struct
    {
        int rawsize = Marshal.SizeOf(value);
        byte[] rawdata = new byte[rawsize];
        GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
        IntPtr buffer = handle.AddrOfPinnedObject();
        Marshal.StructureToPtr(value, buffer, false);
        handle.Free();
        return rawdata;
    }
}
