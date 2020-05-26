using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq.Helpers
{
    /// <summary>
    /// Class TypeHelper.
    /// </summary>
    internal static class TypeHelper
    {
        /// <summary>
        /// Finds the type of the generic.
        /// </summary>
        /// <param name="generic">The generic.</param>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        public static Type FindGenericType(Type generic, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == generic)
                {
                    return type;
                }

                if (generic.GetTypeInfo().IsInterface)
                {
                    foreach (var intfType in type.GetInterfaces())
                    {
                        var found = FindGenericType(generic, intfType);
                        if (found != null) return found;
                    }
                }

                type = type.GetTypeInfo().BaseType;
            }

            return null;
        }

        /// <summary>
        /// Determines whether [is compatible with] [the specified source].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if [is compatible with] [the specified source]; otherwise, <c>false</c>.</returns>
        public static bool IsCompatibleWith(Type source, Type target)
        {
            if (source == target)
            {
                return true;
            }
            if (!target.GetTypeInfo().IsValueType)
            {
                return target.IsAssignableFrom(source);
            }
            var st = GetNonNullableType(source);
            var tt = GetNonNullableType(target);

            if (st != source && tt == target)
            {
                return false;
            }
            var sc = st.GetTypeInfo().IsEnum ? typeof(object) : st;
            var tc = tt.GetTypeInfo().IsEnum ? typeof(object) : tt;

            if (sc == typeof(sbyte))
            {
                if (tc == typeof(sbyte) || tc == typeof(short) || tc == typeof(int) || tc == typeof(long) || tc == typeof(float) || tc == typeof(double) || tc == typeof(decimal))
                    return true;
            }
            else if (sc == typeof(byte))
            {
                if (tc == typeof(byte) || tc == typeof(short) || tc == typeof(ushort) || tc == typeof(int) || tc == typeof(uint) || tc == typeof(long) || tc == typeof(ulong) || tc == typeof(float) || tc == typeof(double) || tc == typeof(decimal))
                    return true;
            }
            else if (sc == typeof(short))
            {
                if (tc == typeof(short) || tc == typeof(int) || tc == typeof(long) || tc == typeof(float) || tc == typeof(double) || tc == typeof(decimal))
                    return true;
            }
            else if (sc == typeof(ushort))
            {
                if (tc == typeof(ushort) || tc == typeof(int) || tc == typeof(uint) || tc == typeof(long) || tc == typeof(ulong) || tc == typeof(float) || tc == typeof(double) || tc == typeof(decimal))
                    return true;
            }
            else if (sc == typeof(int))
            {
                if (tc == typeof(int) || tc == typeof(long) || tc == typeof(float) || tc == typeof(double) || tc == typeof(decimal))
                    return true;
            }
            else if (sc == typeof(uint))
            {
                if (tc == typeof(uint) || tc == typeof(long) || tc == typeof(ulong) || tc == typeof(float) || tc == typeof(double) || tc == typeof(decimal))
                    return true;
            }
            else if (sc == typeof(long))
            {
                if (tc == typeof(long) || tc == typeof(float) || tc == typeof(double) || tc == typeof(decimal))
                    return true;
            }
            else if (sc == typeof(ulong))
            {
                if (tc == typeof(ulong) || tc == typeof(float) || tc == typeof(double) || tc == typeof(decimal))
                    return true;
            }
            else if (sc == typeof(float))
            {
                if (tc == typeof(float) || tc == typeof(double))
                    return true;
            }

            if (st == tt)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is enum type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is enum type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsEnumType(Type type)
        {
            return GetNonNullableType(type).GetTypeInfo().IsEnum;
        }

        /// <summary>
        /// Determines whether [is numeric type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is numeric type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsNumericType(Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        /// <summary>
        /// Determines whether [is nullable type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is nullable type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsNullableType(Type type)
        {
            Argument.IsNotNull(nameof(type), type);

            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Converts to nullabletype.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        public static Type ToNullableType(Type type)
        {
            Argument.IsNotNull(nameof(type), type);

            return IsNullableType(type) ? type : typeof(Nullable<>).MakeGenericType(type);
        }

        /// <summary>
        /// Determines whether [is signed integral type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is signed integral type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsSignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        /// <summary>
        /// Determines whether [is unsigned integral type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is unsigned integral type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsUnsignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        /// <summary>
        /// Gets the kind of the numeric type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Int32.</returns>
        private static int GetNumericTypeKind(Type type)
        {
            type = GetNonNullableType(type);

            if (type.GetTypeInfo().IsEnum)
            {
                return 0;
            }

            if (type == typeof(char) || type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                return 1;
            if (type == typeof(sbyte) || type == typeof(short) || type == typeof(int) || type == typeof(long))
                return 2;
            if (type == typeof(byte) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
                return 3;

            return 0;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        public static string GetTypeName(Type type)
        {
            var baseType = GetNonNullableType(type);

            var name = baseType.Name;
            if (type != baseType)
            {
                name += '?';
            }

            return name;
        }

        /// <summary>
        /// Gets the type of the non nullable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        public static Type GetNonNullableType(Type type)
        {
            Argument.IsNotNull(nameof(type), type);

            return IsNullableType(type) ? type.GetTypeInfo().GenericTypeArguments[0] : type;
        }

        /// <summary>
        /// Gets the type of the underlying.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        public static Type GetUnderlyingType(Type type)
        {
            Argument.IsNotNull(nameof(type), type);

            var genericTypeArguments = type.GetGenericArguments();
            if (genericTypeArguments.Any())
            {
                var outerType = GetUnderlyingType(genericTypeArguments.LastOrDefault());
                return Nullable.GetUnderlyingType(type) == outerType ? type : outerType;
            }

            return type;
        }

        /// <summary>
        /// Gets the self and base types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;Type&gt;.</returns>
        public static IEnumerable<Type> GetSelfAndBaseTypes(Type type)
        {
            if (type.GetTypeInfo().IsInterface)
            {
                var types = new List<Type>();
                AddInterface(types, type);
                return types;
            }
            return GetSelfAndBaseClasses(type);
        }

        /// <summary>
        /// Gets the self and base classes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;Type&gt;.</returns>
        private static IEnumerable<Type> GetSelfAndBaseClasses(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.GetTypeInfo().BaseType;
            }
        }

        /// <summary>
        /// Adds the interface.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="type">The type.</param>
        private static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
                foreach (var t in type.GetInterfaces())
                {
                    AddInterface(types, t);
                }
            }
        }

        /// <summary>
        /// Parses the enum.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public static object ParseEnum(string value, Type type)
        {
            if (type.GetTypeInfo().IsEnum && Enum.IsDefined(type, value))
            {
                return Enum.Parse(type, value, true);
            }

            return null;
        }
    }
}
