using ISynergy.Framework.Core.Attributes;
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
    public static string GetDescription<T>(this T enumerationValue)
        where T : Enum
    {
        Type type = enumerationValue.GetType();

        if (!type.IsEnum)
            throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");

        //Tries to find a DescriptionAttribute for a potential friendly name
        //for the enum
        MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
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
    public static string GetLocalizedDescription<T>(this T enumerationValue)
        where T : Enum
    {
        Type type = enumerationValue.GetType();
        if (!type.IsEnum)
            throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");

        //Tries to find a DescriptionAttribute for a potential friendly name
        //for the enum
        MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
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
    public static List<Enum> ToList(this Type enumeration)
    {
        var list = new List<Enum>();

        if (enumeration.IsEnum)
            foreach (Enum item in Enum.GetValues(enumeration))
                list.Add(item);

        return list;
    }
}
