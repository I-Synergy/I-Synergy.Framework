using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Object extensions providing utility methods for type conversion and reflection.
/// </summary>
public static class ObjectExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Perform a deep copy of the object using JSON serialization.
    /// </summary>
    /// <typeparam name="T">The type of object being copied.</typeparam>
    /// <param name="source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    /// <exception cref="InvalidOperationException">Thrown when source is null.</exception>
    public static T Clone<T>(this T source)
    {
        if (source is null)
            throw new InvalidOperationException("Source cannot be null");

        var serialized = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(serialized, DefaultJsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize cloned object");
    }

    /// <summary>
    /// Checks if type is a nullable type.
    /// </summary>
    /// <typeparam name="T">A type object.</typeparam>
    /// <param name="self">The type to check.</param>
    /// <returns><c>true</c> if the type is nullable; otherwise, <c>false</c>.</returns>
    public static bool IsNullableType<T>(this T self) where T : Type =>
        !self.IsValueType || Nullable.GetUnderlyingType(self) is not null;

    /// <summary>
    /// Converts an object into another type, irrespective of whether
    /// the conversion can be done at compile time or not.
    /// </summary>
    /// <typeparam name="T">The destination type.</typeparam>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The result of the conversion or null if conversion fails.</returns>
    public static T? To<T>(this object? value) =>
        (T?)To(value, typeof(T));

    /// <summary>
    /// Converts an object into another type, irrespective of whether
    /// the conversion can be done at compile time or not.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <param name="type">The type that the value should be converted to.</param>
    /// <returns>The result of the conversion or null if conversion fails.</returns>
    public static object? To(this object? value, Type type)
    {
        if (value is null)
            return type.IsValueType ? Activator.CreateInstance(type) : null;

        if (type.IsInstanceOfType(value))
            return value;

        // Handle enum conversion
        if (type.GetTypeInfo().IsEnum)
        {
            var numericValue = System.Convert.ChangeType(value, typeof(int));
            return Enum.ToObject(type, numericValue);
        }

        var inputType = value.GetType();

        // Handle Nullable<T>
        if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var implicitMethod = type.GetMethod("op_Implicit", [inputType]);
            if (implicitMethod is not null)
                return implicitMethod.Invoke(null, [value]);
        }

        // Look for explicit/implicit conversion operators
        var conversionMethod = FindConversionMethod(inputType, type, value);
        if (conversionMethod is not null)
            return conversionMethod.Invoke(null, [value]);

        // Fall back to ChangeType
        return System.Convert.ChangeType(value, type);
    }

    /// <summary>
    /// Checks whether an object implements a method with the given name.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <returns><c>true</c> if the method exists; otherwise, <c>false</c>.</returns>
    public static bool HasMethod(this object? obj, string methodName)
    {
        if (obj is null)
            return false;

        try
        {
            return obj.GetType().GetMethod(methodName) is not null;
        }
        catch (AmbiguousMatchException)
        {
            // Multiple methods with same name exist, which means method does exist
            return true;
        }
    }

    /// <summary>
    /// Determines whether the object has a property with the specified name.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns><c>true</c> if the property exists; otherwise, <c>false</c>.</returns>
    public static bool HasProperty(this object? obj, string propertyName)
    {
        if (obj is null)
            return false;

        try
        {
            return obj.GetType().GetRuntimeProperty(propertyName) is not null;
        }
        catch (AmbiguousMatchException)
        {
            // Multiple properties with same name exist, which means property does exist
            return true;
        }
    }

    /// <summary>
    /// Determines whether <c>a > b</c>.
    /// </summary>
    /// <typeparam name="T">Type implementing IComparable.</typeparam>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <returns><c>true</c> if a is greater than b; otherwise, <c>false</c>.</returns>
    public static bool IsGreaterThan<T>(this T a, object? b) where T : IComparable =>
        a.CompareTo(b) > 0;

    /// <summary>
    /// Determines whether <c>a >= b</c>.
    /// </summary>
    /// <typeparam name="T">Type implementing IComparable.</typeparam>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <returns><c>true</c> if a is greater than or equal to b; otherwise, <c>false</c>.</returns>
    public static bool IsGreaterThanOrEqual<T>(this T a, object? b) where T : IComparable =>
        a.CompareTo(b) >= 0;

    /// <summary>
    /// Determines whether <c>a &lt; b</c>.
    /// </summary>
    /// <typeparam name="T">Type implementing IComparable.</typeparam>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <returns><c>true</c> if a is less than b; otherwise, <c>false</c>.</returns>
    public static bool IsLessThan<T>(this T a, object? b) where T : IComparable =>
        a.CompareTo(b) < 0;

    /// <summary>
    /// Determines whether <c>a &lt;= b</c>.
    /// </summary>
    /// <typeparam name="T">Type implementing IComparable.</typeparam>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <returns><c>true</c> if a is less than or equal to b; otherwise, <c>false</c>.</returns>
    public static bool IsLessThanOrEqual<T>(this T a, object? b) where T : IComparable =>
        a.CompareTo(b) <= 0;

    /// <summary>
    /// Serializes (converts) a structure to a byte array using marshaling.
    /// </summary>
    /// <typeparam name="T">The structure type to serialize.</typeparam>
    /// <param name="value">The structure to be serialized.</param>
    /// <returns>The byte array containing the serialized structure.</returns>
    public static byte[] ToByteArray<T>(this T value) where T : struct
    {
        var size = Marshal.SizeOf(value);
        var rawData = new byte[size];
        var handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);

        try
        {
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            return rawData;
        }
        finally
        {
            handle.Free();
        }
    }

    /// <summary>
    /// Finds an explicit or implicit conversion method between types.
    /// </summary>
    private static MethodInfo? FindConversionMethod(Type inputType, Type outputType, object value)
    {
        var methods = new List<MethodInfo>();
        methods.AddRange(inputType.GetMethods(BindingFlags.Public | BindingFlags.Static));
        methods.AddRange(outputType.GetMethods(BindingFlags.Public | BindingFlags.Static));

        foreach (var method in methods)
        {
            if (!method.IsPublic || !method.IsStatic)
                continue;

            if ((method.Name != "op_Implicit" && method.Name != "op_Explicit") || method.ReturnType != outputType)
                continue;

            var parameters = method.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType.IsInstanceOfType(value))
                return method;
        }

        return null;
    }
}
