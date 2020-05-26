using ISynergy.Framework.Core.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace ISynergy.Framework.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static string GetIdentityPropertyName<T>() where T : class
        {
            var result = typeof(T).GetProperties().Where(
                    e => e.IsDefined(typeof(IdentityAttribute))
                );

            if (result.Count() > 0)
            {
                return result.First().Name;
            }
            else
            {
                return null;
            }
        }

        public static object GetIdentityValue<T>(this T _self) where T : class
        {
            var result = _self.GetType().GetProperties().Where(
                    e => e.IsDefined(typeof(IdentityAttribute))
                );

            if (result.Count() > 0)
            {
                return result.First().GetValue(_self);
            }
            else
            {
                return null;
            }
        }

        public static PropertyInfo GetIdentityProperty<T>() where T : class
        {
            return typeof(T).GetProperties().FirstOrDefault(e => e.IsDefined(typeof(IdentityAttribute)));
        }

        public static PropertyInfo GetIdentityProperty<T>(this T _self) where T : class
        {
            return _self.GetIdentityProperty();
        }

        public static bool HasIdentityProperty<T>() where T : class
        {
            return typeof(T).GetProperties().Any(e => e.IsDefined(typeof(IdentityAttribute)));
        }

        public static bool HasIdentityProperty<T>(this T _self) where T : class
        {
            return _self.HasIdentityProperty();
        }

        public static string GetParentIdentityPropertyName<T>() where T : class
        {
            var result = typeof(T).GetProperties().Where(
                    e => e.IsDefined(typeof(ParentIdentityAttribute))
                );

            if (result.Count() > 0)
            {
                return result.First().Name;
            }
            else
            {
                return null;
            }
        }

        public static Type GetParentIdentityPropertyType<T>() where T : class
        {
            var result = typeof(T).GetProperties().Where(
                    e => e.IsDefined(typeof(ParentIdentityAttribute))
                );

            if (result.Count() > 0)
            {
                return result.First().PropertyType;
            }
            else
            {
                return typeof(object);
            }
        }

        public static TResult GetPropertyValue<T, TResult>(this T _self, string propertyName, TResult defaultValue)
            where T : class
            where TResult : IComparable<TResult>
        {
            var propInfo = _self.GetType().GetProperty(propertyName);
            var prop = propInfo.GetValue(_self, null);

            if(prop is TResult result)
            {
                return result;
            }

            return defaultValue;
        }
    }
}
