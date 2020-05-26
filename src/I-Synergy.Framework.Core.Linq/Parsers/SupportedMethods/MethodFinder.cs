using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ISynergy.Framework.Core.Linq.Helpers;

namespace ISynergy.Framework.Core.Linq.Parsers.SupportedMethods
{
    internal class MethodFinder
    {
        private readonly ParsingConfig _parsingConfig;

        /// <summary>
        /// Get an instance
        /// </summary>
        /// <param name="parsingConfig"></param>
        public MethodFinder(ParsingConfig parsingConfig)
        {
            _parsingConfig = parsingConfig;
        }

        public bool ContainsMethod(Type type, string methodName, bool staticAccess, Expression[] args)
        {
            return FindMethod(type, methodName, staticAccess, args, out _) == 1;
        }

        public int FindMethod(Type type, string methodName, bool staticAccess, Expression[] args, out MethodBase method)
        {
            foreach (var t in SelfAndBaseTypes(type))
            {
                var methods = t.GetTypeInfo().DeclaredMethods.Where(x => (x.IsStatic || !staticAccess) && x.Name.ToLowerInvariant() == methodName.ToLowerInvariant()).ToArray();
                var count = FindBestMethod(methods, args, out method);
                if (count != 0)
                {
                    return count;
                }
            }
            method = null;
            return 0;
        }

        public int FindBestMethod(IEnumerable<MethodBase> methods, Expression[] args, out MethodBase method)
        {
            var applicable = methods.
                Select(m => new MethodData { MethodBase = m, Parameters = m.GetParameters() }).
                Where(m => IsApplicable(m, args)).
                ToArray();

            if (applicable.Length > 1)
            {
                applicable = applicable.Where(m => applicable.All(n => m == n || IsBetterThan(args, m, n))).ToArray();
            }

            if (args.Length == 2 && applicable.Length > 1 && (args[0].Type == typeof(Guid?) || args[1].Type == typeof(Guid?)))
            {
                applicable = applicable.Take(1).ToArray();
            }

            if (applicable.Length == 1)
            {
                var md = applicable[0];
                for (var i = 0; i < args.Length; i++)
                {
                    args[i] = md.Args[i];
                }
                method = md.MethodBase;
            }
            else
            {
                method = null;
            }

            return applicable.Length;
        }

        public int FindIndexer(Type type, Expression[] args, out MethodBase method)
        {
            foreach (var t in SelfAndBaseTypes(type))
            {
                var members = t.GetDefaultMembers();
                if (members.Length != 0)
                {
                    var methods = members.OfType<PropertyInfo>().
#if !(NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
                        Select(p => (MethodBase)p.GetGetMethod()).
                        Where(m => m != null);
#else
                    Select(p => (MethodBase)p.GetMethod);
#endif
                    var count = FindBestMethod(methods, args, out method);
                    if (count != 0)
                    {
                        return count;
                    }
                }
            }

            method = null;
            return 0;
        }

        bool IsApplicable(MethodData method, Expression[] args)
        {
            if (method.Parameters.Length != args.Length)
            {
                return false;
            }

            var promotedArgs = new Expression[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                var pi = method.Parameters[i];
                if (pi.IsOut)
                {
                    return false;
                }

                var promoted = this._parsingConfig.ExpressionPromoter.Promote(args[i], pi.ParameterType, false, method.MethodBase.DeclaringType != typeof(IEnumerableSignatures));
                if (promoted == null)
                {
                    return false;
                }
                promotedArgs[i] = promoted;
            }
            method.Args = promotedArgs;
            return true;
        }

        bool IsBetterThan(Expression[] args, MethodData first, MethodData second)
        {
            var better = false;
            for (var i = 0; i < args.Length; i++)
            {
                var result = CompareConversions(args[i].Type, first.Parameters[i].ParameterType, second.Parameters[i].ParameterType);

                // If second is better, return false
                if (result == CompareConversionType.Second)
                {
                    return false;
                }

                // If first is better, return true
                if (result == CompareConversionType.First)
                {
                    return true;
                }

                // If both are same, just set better to true and continue
                if (result == CompareConversionType.Both)
                {
                    better = true;
                }
            }

            return better;
        }

        // Return "First" if s -> t1 is a better conversion than s -> t2
        // Return "Second" if s -> t2 is a better conversion than s -> t1
        // Return "Both" if neither conversion is better
        CompareConversionType CompareConversions(Type source, Type first, Type second)
        {
            if (first == second)
            {
                return CompareConversionType.Both;
            }
            if (source == first)
            {
                return CompareConversionType.First;
            }
            if (source == second)
            {
                return CompareConversionType.Second;
            }

            var firstIsCompatibleWithSecond = TypeHelper.IsCompatibleWith(first, second);
            var secondIsCompatibleWithFirst = TypeHelper.IsCompatibleWith(second, first);

            if (firstIsCompatibleWithSecond && !secondIsCompatibleWithFirst)
            {
                return CompareConversionType.First;
            }
            if (secondIsCompatibleWithFirst && !firstIsCompatibleWithSecond)
            {
                return CompareConversionType.Second;
            }

            if (TypeHelper.IsSignedIntegralType(first) && TypeHelper.IsUnsignedIntegralType(second))
            {
                return CompareConversionType.First;
            }
            if (TypeHelper.IsSignedIntegralType(second) && TypeHelper.IsUnsignedIntegralType(first))
            {
                return CompareConversionType.Second;
            }

            return CompareConversionType.Both;
        }

        IEnumerable<Type> SelfAndBaseTypes(Type type)
        {
            if (type.GetTypeInfo().IsInterface)
            {
                var types = new List<Type>();
                AddInterface(types, type);
                return types;
            }
            return SelfAndBaseClasses(type);
        }

        IEnumerable<Type> SelfAndBaseClasses(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.GetTypeInfo().BaseType;
            }
        }

        void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
                foreach (var t in type.GetInterfaces()) AddInterface(types, t);
            }
        }
    }
}
