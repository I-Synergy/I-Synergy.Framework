using ISynergy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ISynergy
{
    public static class Reflection
    {
        public static string GetIdentityPropertyName<T>() where T : class
        {
            IEnumerable<PropertyInfo> result = typeof(T).GetProperties().Where(
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

        public static object GetIdentityValue<T>(T t) where T : class
        {
            IEnumerable<PropertyInfo> result = t.GetType().GetProperties().Where(
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
            IEnumerable<PropertyInfo> result = typeof(T).GetProperties().Where(
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
            IEnumerable<PropertyInfo> result = typeof(T).GetProperties().Where(
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

        public static TResult GetPropertyValue<T, TResult>(T t, string propertyName, TResult defaultvalue)
            where T : class
            where TResult : IComparable<TResult>
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            UnaryExpression expression = Expression.Convert(Expression.Property(parameter, propertyName), typeof(object));

            dynamic resolver = Expression.Lambda<Func<T, TResult>>(expression, parameter).Compile();

            return resolver(t);
        }
    }
}