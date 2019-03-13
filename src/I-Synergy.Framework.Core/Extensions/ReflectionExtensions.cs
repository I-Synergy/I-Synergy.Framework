using ISynergy.Attributes;
using System.Linq;
using System.Reflection;

namespace System
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

        public static object GetIdentityValue<T>(this T t) where T : class
        {
            var result = t.GetType().GetProperties().Where(
                    e => e.IsDefined(typeof(IdentityAttribute))
                );

            if (result.Count() > 0)
            {
                return result.First().GetValue(t);
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

        public static TResult GetPropertyValue<T, TResult>(this T t, string propertyName, TResult defaultValue)
            where T : class
            where TResult : IComparable<TResult>
        {
            var result = (TResult)t.GetType().GetProperty(propertyName)?.GetValue(t, null);

            if(result == null)
            {
                result = defaultValue;
            }

            return result;
        }
    }
}