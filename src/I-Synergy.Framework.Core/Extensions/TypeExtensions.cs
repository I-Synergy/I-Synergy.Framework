using System;
using System.Collections.Generic;
using System.Linq;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Get interfaces from type.
        /// </summary>
        /// <param name="_self"></param>
        /// <param name="includeInherited"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetInterfaces(this Type _self, bool includeInherited)
        {
            if (includeInherited || _self.BaseType == null)
                return _self.GetInterfaces();
            else
                return _self.GetInterfaces().Except(_self.BaseType.GetInterfaces());
        }
    }
}
