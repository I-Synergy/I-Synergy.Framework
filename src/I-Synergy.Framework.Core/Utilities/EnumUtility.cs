using ISynergy.Framework.Core.Exceptions;
using ISynergy.Framework.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Core.Utilities
{
    public static class EnumUtility
    {
        /// <summary>
        /// If <see cref="Enum.IsDefined(Type, object)"/> returns false, this
        /// method throw a <see cref="UnexpectedEnumValueException"/>.
        /// </summary>
        public static void ThrowIfUndefined(Type enumType, object value)
        {
            if (!Enum.IsDefined(enumType, value))
                throw new UnexpectedEnumValueException(enumType, value);
        }

        public static bool TryToEnum<T>(string str, out T result)
            where T : struct
        {
            var enumType = typeof(T);

            foreach (var name in Enum.GetNames(enumType).EnsureNotNull())
            {
                var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetRuntimeField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();

                if (enumMemberAttribute.Value == str)
                {
                    result = (T)Enum.Parse(enumType, name, true);
                    return true;
                }
            }

            result = default;
            return false;
        }

        public static IEnumerable<Enum> GetUniqueFlags(this Enum flags)
        {
            ulong flag = 1;

            foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>().EnsureNotNull())
            {
                var bits = Convert.ToUInt64(value);

                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }
    }
}
