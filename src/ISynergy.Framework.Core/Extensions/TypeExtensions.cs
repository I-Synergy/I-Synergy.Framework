using System.Reflection;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Type extensions.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Get interfaces from type.
    /// </summary>
    /// <param name="_self">The self.</param>
    /// <param name="includeInherited">if set to <c>true</c> [include inherited].</param>
    /// <returns>IEnumerable&lt;Type&gt;.</returns>
    public static IEnumerable<Type> GetInterfaces(this Type _self, bool includeInherited)
    {
        if (includeInherited || _self.BaseType is null)
            return _self.GetInterfaces();
        return _self.GetInterfaces().Except(_self.BaseType.GetInterfaces());
    }

    /// <summary>
    /// Gets the type of the element in a jagged or multi-dimensional matrix.
    /// </summary>
    /// 
    /// <param name="type">The array type whose element type should be computed.</param>
    /// 
    public static Type GetInnerMostType(this Type type)
    {
        while (type.IsArray)
            type = type.GetElementType()!;

        return type;
    }

    /// <summary>
    ///   Determines whether the given type has a public default (parameterless) constructor.
    /// </summary>
    /// 
    /// <param name="t">The type to check.</param>
    /// 
    /// <returns>True if the type has a public parameterless constructor; false otherwise.</returns>
    /// 
    public static bool HasDefaultConstructor(this Type t)
    {
        return t.IsValueType || t.GetConstructor(Type.EmptyTypes) is not null;
    }

    /// <summary>
    ///   Gets the default value for a type. This method should serve as
    ///   a programmatic equivalent to <c>default(T)</c>.
    /// </summary>
    /// 
    /// <param name="type">The type whose default value should be retrieved.</param>
    /// 
    public static object? GetDefaultValue(this Type type)
    {
        if (type.GetTypeInfo().IsValueType)
            return Activator.CreateInstance(type);
        return null;
    }

    /// <summary>
    ///   Returns a type object representing an array of the current type, with the specified number of dimensions.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <param name="rank">The rank.</param>
    /// <param name="jagged">Whether to return a type for a jagged array of the given rank, or a 
    /// multdimensional array. Default is false (default is to return multidimensional array types).</param>
    /// 
    public static Type MakeArrayType(this Type elementType, int rank, bool jagged)
    {
        if (!jagged)
            return elementType.MakeArrayType(rank);

        if (rank == 0)
            return elementType;

        return elementType.MakeArrayType(rank: rank - 1, jagged: true).MakeArrayType();
    }
}
