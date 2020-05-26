using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq.Parsers
{
    internal static class PredefinedTypesHelper
    {
        // These shorthands have different name than actual type and therefore not recognized by default from the PredefinedTypes.
        public static readonly IDictionary<string, Type> PredefinedTypesShorthands = new Dictionary<string, Type>
        {
            { "int", typeof(int) },
            { "uint", typeof(uint) },
            { "short", typeof(short) },
            { "ushort", typeof(ushort) },
            { "long", typeof(long) },
            { "ulong", typeof(ulong) },
            { "bool", typeof(bool) },
            { "float", typeof(float) }
        };

        public static readonly IDictionary<Type, int> PredefinedTypes = new ConcurrentDictionary<Type, int>(new Dictionary<Type, int> {
            { typeof(object), 0 },
            { typeof(bool), 0 },
            { typeof(char), 0 },
            { typeof(string), 0 },
            { typeof(sbyte), 0 },
            { typeof(byte), 0 },
            { typeof(short), 0 },
            { typeof(ushort), 0 },
            { typeof(int), 0 },
            { typeof(uint), 0 },
            { typeof(long), 0 },
            { typeof(ulong), 0 },
            { typeof(float), 0 },
            { typeof(double), 0 },
            { typeof(decimal), 0 },
            { typeof(DateTime), 0 },
            { typeof(DateTimeOffset), 0 },
            { typeof(TimeSpan), 0 },
            { typeof(Guid), 0 },
            { typeof(Math), 0 },
            { typeof(Convert), 0 },
            { typeof(Uri), 0 }
        });

        private static void TryAdd(string typeName, int x)
        {
            try
            {
                var efType = Type.GetType(typeName);
                if (efType != null)
                {
                    PredefinedTypes.Add(efType, x);
                }
            }
            catch
            {
                // in case of exception, do not add
            }
        }

        public static bool IsPredefinedType(ParsingConfig config, Type type)
        {
            Argument.IsNotNull(nameof(config), config);
            Argument.IsNotNull(nameof(type), type);

            if (PredefinedTypes.ContainsKey(type))
            {
                return true;
            }

            return config.CustomTypeProvider != null && config.CustomTypeProvider.GetCustomTypes().Contains(type);
        }
    }
}
