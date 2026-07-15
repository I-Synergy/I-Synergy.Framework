using ISynergy.Framework.Core.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Class EnumExtensions.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerationValue">The enumeration value.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="ArgumentException">EnumerationValue must be of Enum type - enumerationValue</exception>
    [UnconditionalSuppressMessage("Trimming", "IL2075:UnrecognizedReflectionPattern",
        Justification = "GetType() is deliberate: typeof(T) resolves to System.Enum when called on an Enum-typed variable. The trimmer preserves an enum type together with its fields and their attributes whenever the enum is preserved, so GetMember on the value runtime type is safe.")]
    public static string GetDescription<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents)] T>(this T enumerationValue)
        where T : Enum
    {
        // Use the concrete runtime type (e.g. Themes) rather than typeof(T)
        // which resolves to System.Enum when called on an Enum-typed variable.
        Type concreteType = enumerationValue.GetType();

        //Tries to find a DescriptionAttribute for a potential friendly name
        //for the enum
        MemberInfo[] memberInfo = concreteType.GetMember(enumerationValue.ToString());
        if (memberInfo is not null && memberInfo.Length > 0)
        {
            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs is not null && attrs.Length > 0)
                return ((DescriptionAttribute)attrs[0]).Description;
        }

        //If we have no description attribute, just return the ToString of the enum
        return enumerationValue.ToString();
    }

    /// <summary>
    /// Gets the localized description.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerationValue">The enumeration value.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="ArgumentException">EnumerationValue must be of Enum type - enumerationValue</exception>
    [UnconditionalSuppressMessage("Trimming", "IL2075:UnrecognizedReflectionPattern",
        Justification = "GetType() is deliberate: typeof(T) resolves to System.Enum when called on an Enum-typed variable. The trimmer preserves an enum type together with its fields and their attributes whenever the enum is preserved, so GetMember on the value runtime type is safe.")]
    public static string GetLocalizedDescription<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents)] T>(this T enumerationValue)
        where T : Enum
    {
        // Use the concrete runtime type (e.g. Themes) rather than typeof(T)
        // which resolves to System.Enum when called on an Enum-typed variable.
        Type concreteType = enumerationValue.GetType();

        //Tries to find a LocalizedDescriptionAttribute for a potential friendly name
        //for the enum
        MemberInfo[] memberInfo = concreteType.GetMember(enumerationValue.ToString());
        if (memberInfo is not null && memberInfo.Length > 0)
        {
            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(LocalizedDescriptionAttribute), false);

            if (attrs is not null && attrs.Length > 0)
                return ((LocalizedDescriptionAttribute)attrs[0]).Description;
        }

        //If we have no description attribute, just return the ToString of the enum
        return enumerationValue.ToString();
    }

    /// <summary>
    /// Converts enum to a list of enum values.
    /// </summary>
    /// <param name="enumeration"></param>
    /// <returns></returns>
    public static List<Enum> ToList([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] this Type enumeration)
    {
        var list = new List<Enum>();

        if (enumeration.IsEnum)
            list.AddRange(Enum.GetValues(enumeration).Cast<Enum>());

        return list;
    }
}
