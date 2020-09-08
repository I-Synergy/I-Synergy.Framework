using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Parsers
{
    /// <summary>
    /// Class EnumerationsFromMscorlib.
    /// </summary>
    internal static class EnumerationsFromMscorlib
    {
        /// <summary>
        /// Some enumeration types from mscorlib/netstandard.
        /// </summary>
        public static readonly IDictionary<Type, int> PredefinedEnumerationTypes = new ConcurrentDictionary<Type, int>(new Dictionary<Type, int> {
            { typeof(AttributeTargets), 0 },
            { typeof(DateTimeKind), 0 },
            { typeof(DayOfWeek), 0 },
            { typeof(GCCollectionMode), 0 },
            { typeof(MidpointRounding), 0 },
            { typeof(StringComparison), 0 },
            { typeof(StringSplitOptions), 0 },
            { typeof(TypeCode), 0 }
        });
    }
}
