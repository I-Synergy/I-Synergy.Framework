using ISynergy.Framework.Core.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ISynergy.Framework.Physics.Extensions;

/// <summary>
/// Enumeration extensions.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the Symbol.
    /// </summary>
    /// <typeparam name="T">The enum type to retrieve the symbol for.</typeparam>
    /// <param name="enumerationValue">The enumeration value.</param>
    /// <returns>
    /// The symbol string defined by <see cref="SymbolAttribute"/> on the enum member, or the
    /// result of <c>ToString()</c> if no <see cref="SymbolAttribute"/> is present.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="enumerationValue"/> is not an enum type.</exception>
    /// <remarks>
    /// <para>
    ///   This method uses reflection (<c>Type.GetMember</c> and
    ///   <c>MemberInfo.GetCustomAttributes</c>) to retrieve the
    ///   <see cref="SymbolAttribute"/> at runtime. This pattern is not compatible with Native AOT
    ///   or aggressive trimming because the trimmer may remove custom attribute metadata for enum
    ///   members of types that are not statically referenced.
    /// </para>
    /// <para>
    ///   For AOT-safe scenarios, replace calls to this method with a static dictionary lookup
    ///   that maps each enum value to its symbol string using compile-time-known string literals.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("GetSymbol uses reflection to read SymbolAttribute from enum members. Use a static lookup table for AOT-compatible scenarios.")]
    public static string GetSymbol<T>(this T enumerationValue)
        where T : struct
    {
        Type type = enumerationValue.GetType();

        if (!type.IsEnum)
            throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");

        //Tries to find a DescriptionAttribute for a potential friendly name
        //for the enum
        MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString()!);

        if (memberInfo is not null && memberInfo.Length > 0)
        {
            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(SymbolAttribute), false);

            if (attrs is not null && attrs.Length > 0)
                return ((SymbolAttribute)attrs[0]).Symbol;
        }

        //If we have no description attribute, just return the ToString of the enum
        return enumerationValue.ToString()!;
    }
}
