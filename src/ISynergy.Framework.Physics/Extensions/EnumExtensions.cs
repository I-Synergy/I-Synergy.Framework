using ISynergy.Framework.Core.Attributes;
using System.Reflection;

namespace ISynergy.Framework.Physics.Extensions
{
    /// <summary>
    /// Enumeration extensions.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the Symbol.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerationValue">The enumeration value.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">EnumerationValue must be of Enum type - enumerationValue</exception>
        public static string GetSymbol<T>(this T enumerationValue)
            where T : struct
        {
            Type type = enumerationValue.GetType();

            if (!type.IsEnum)
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());

            if (memberInfo is not null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(SymbolAttribute), false);

                if (attrs is not null && attrs.Length > 0)
                    return ((SymbolAttribute)attrs[0]).Symbol;
            }

            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
    }
}
