using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class ReflectionExtensions.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the name of the identity property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.String.</returns>
        public static string GetIdentityPropertyName<T>() where T : class
        {
            var result = typeof(T).GetProperties().Where(
                    e => e.IsDefined(typeof(IdentityAttribute)));

            if (result.Any())
            {
                return result.First().Name;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the identity value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_self">The self.</param>
        /// <returns>System.Object.</returns>
        public static object GetIdentityValue<T>(this T _self) where T : class
        {
            var result = _self.GetType().GetProperties().Where(
                    e => e.IsDefined(typeof(IdentityAttribute))
                );

            if (result.Any())
            {
                return result.First().GetValue(_self);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the identity property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>PropertyInfo.</returns>
        public static PropertyInfo GetIdentityProperty<T>() where T : class
        {
            return typeof(T).GetProperties().FirstOrDefault(e => e.IsDefined(typeof(IdentityAttribute)));
        }

        /// <summary>
        /// Gets the identity property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_self">The self.</param>
        /// <returns>PropertyInfo.</returns>
        public static PropertyInfo GetIdentityProperty<T>(this T _self) where T : class
        {
            var result = _self.GetType().GetProperties().Where(
                    e => e.IsDefined(typeof(IdentityAttribute))
                );

            if (result.Any())
            {
                return result.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Determines whether [has identity property].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><c>true</c> if [has identity property]; otherwise, <c>false</c>.</returns>
        public static bool HasIdentityProperty<T>() where T : class
        {
            return typeof(T).GetProperties().Any(e => e.IsDefined(typeof(IdentityAttribute)));
        }

        /// <summary>
        /// Determines whether [has identity property] [the specified self].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_self">The self.</param>
        /// <returns><c>true</c> if [has identity property] [the specified self]; otherwise, <c>false</c>.</returns>
        public static bool HasIdentityProperty<T>(this T _self) where T : class
        {
            return _self.GetType().GetProperties().Any(e => e.IsDefined(typeof(IdentityAttribute)));
        }

        /// <summary>
        /// Gets the name of the parent identity property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.String.</returns>
        public static string GetParentIdentityPropertyName<T>() where T : class
        {
            var result = typeof(T).GetProperties().Where(
                    e => e.IsDefined(typeof(ParentIdentityAttribute))
                );

            if (result.Any())
            {
                return result.First().Name;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the type of the parent identity property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Type.</returns>
        public static Type GetParentIdentityPropertyType<T>() where T : class
        {
            var result = typeof(T).GetProperties().Where(
                    e => e.IsDefined(typeof(ParentIdentityAttribute))
                );

            if (result.Any())
            {
                return result.First().PropertyType;
            }
            else
            {
                return typeof(object);
            }
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="_self">The self.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>TResult.</returns>
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

        /// <summary>
        /// GetPropertyInfo extension.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="_self"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T, TValue>(this T _self, Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;

            if (body is LambdaExpression)
                body = ((LambdaExpression)body).Body;

            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return (PropertyInfo)((MemberExpression)body).Member;
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// GetProperty extension.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="_self"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Property<TValue> GetProperty<T, TValue>(this T _self, Expression<Func<T, TValue>> selector)
            where T : class, IObservableClass
        {
            Expression body = selector;

            if (body is LambdaExpression)
                body = ((LambdaExpression)body).Body;

            if (body.NodeType == ExpressionType.MemberAccess)
            {
                var key = ((MemberExpression)body).Member.Name;
                return _self.Properties[key] as Property<TValue>;
            }

            throw new InvalidOperationException();
        }
    }
}
