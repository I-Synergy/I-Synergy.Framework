using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace ISynergy.Framework.Core.Extensions;

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
    public static string? GetIdentityPropertyName<T>() where T : class
    {
        var result = typeof(T).GetProperties().Where(
                e => e.IsDefined(typeof(IdentityAttribute)));

        if (result.Any())
            return result.First().Name;

        return null;
    }

    /// <summary>
    /// Gets the identity value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_self">The self.</param>
    /// <returns>System.Object.</returns>
    public static object? GetIdentityValue<T>(this T _self) where T : class
    {
        var result = _self.GetType().GetProperties().Where(
                e => e.IsDefined(typeof(IdentityAttribute))
            );

        if (result.Any())
            return result.First().GetValue(_self);

        return null;
    }

    /// <summary>
    /// Gets the identity value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="_self">The self.</param>
    /// <returns>T.</returns>
    public static TResult GetIdentityValue<T, TResult>(this T _self)
        where T : class
        where TResult : struct
    {
        var result = _self.GetType().GetProperties().Where(
                e => e.IsDefined(typeof(IdentityAttribute))
            );

        if (result.Any())
        {
            var value = result.First().GetValue(_self);

            // If value is null, return default value for TResult
            if (value is null)
                return default;

            // If value is already of type TResult, return it directly
            if (value is TResult typedValue)
                return typedValue;

            // Otherwise, try to convert it to TResult
            return (TResult)Convert.ChangeType(value, typeof(TResult));
        }

        return default;
    }

    /// <summary>
    /// Gets the identity property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>PropertyInfo.</returns>
    public static PropertyInfo? GetIdentityProperty<T>() where T : class
    {
        return typeof(T).GetProperties().FirstOrDefault(e => e.IsDefined(typeof(IdentityAttribute)));
    }

    /// <summary>
    /// Gets the identity property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_self">The self.</param>
    /// <returns>PropertyInfo.</returns>
    public static PropertyInfo? GetIdentityProperty<T>(this T _self) where T : class
    {
        var result = _self.GetType().GetProperties().Where(
                e => e.IsDefined(typeof(IdentityAttribute))
            );

        if (result.Any())
            return result.First();

        return null;
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
    public static string? GetParentIdentityPropertyName<T>() where T : class
    {
        var result = typeof(T).GetProperties().Where(
                e => e.IsDefined(typeof(ParentIdentityAttribute))
            );

        if (result.Any())
            return result.First().Name;

        return null;
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
            return result.First().PropertyType;
        return typeof(object);
    }

    /// <summary>
    /// Gets the property value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="_self">The self.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>T.</returns>
    public static TResult GetPropertyValue<T, TResult>(this T _self, string propertyName, TResult defaultValue)
        where T : class
        where TResult : IComparable<TResult>
    {
        var propInfo = _self.GetType().GetProperty(propertyName);
        var prop = propInfo?.GetValue(_self, null);

        if (prop is TResult result)
            return result;

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
    public static Property<TValue>? GetProperty<T, TValue>(this T _self, Expression<Func<T, TValue>> selector)
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

    /// <summary>
    /// Gets the identity value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_self">The self.</param>
    /// <returns>System.Object.</returns>
    public static string GetTitleValue<T>(this T _self) where T : class
    {
        var result = _self.GetType().GetProperties().Where(
                e => e.IsDefined(typeof(TitleAttribute))
            );

        if (result is not null && result.Count() > 0)
            return result.First().GetValue(_self)?.ToString() ?? string.Empty;

        return string.Empty;
    }

    public static bool HasParentIdentityProperty<T>() where T : class =>
        typeof(T).HasParentIdentityProperty();

    public static bool HasParentIdentityProperty<T>(this T _self) where T : class =>
        _self.GetType().GetProperties().Any(e => e.IsDefined(typeof(ParentIdentityAttribute)));

    public static PropertyInfo GetParentIdentityProperty<T>() where T : class =>
        typeof(T).GetParentIdentityProperty();

    public static PropertyInfo GetParentIdentityProperty<T>(this T _self)
        where T : class
    {
        var property = _self.GetType().GetProperties().FirstOrDefault(e => e.IsDefined(typeof(ParentIdentityAttribute)));

        if (property is null)
            throw new InvalidOperationException("Parent identity property not found. Check with HasParentIdentityProperty first.");

        return property;
    }

    /// <summary>
    /// Gets the identity value.
    /// </summary>
    /// <param name="_self"></param>
    /// <returns>System.Object.</returns>
    public static bool IsFreeApplication(this Type _self)
    {
        if (Attribute.GetCustomAttribute(_self, typeof(FreeAttribute)) is FreeAttribute attribute)
            return attribute.IsFree;
        return false;
    }

    /// <summary>
    /// Check if class has singleton attribute.    
    /// </summary>
    /// <param name="_self"></param>
    /// <returns></returns>
    public static bool IsSingleton(this Type _self)
    {
        if (Attribute.GetCustomAttribute(_self, typeof(LifetimeAttribute)) is LifetimeAttribute attribute)
            return attribute.Lifetime == Lifetimes.Singleton;
        return false;
    }

    /// <summary>
    /// Check if class has scoped attribute.    
    /// </summary>
    /// <param name="_self"></param>
    /// <returns></returns>
    public static bool IsScoped(this Type _self)
    {
        if (Attribute.GetCustomAttribute(_self, typeof(LifetimeAttribute)) is LifetimeAttribute attribute)
            return attribute.Lifetime == Lifetimes.Scoped;
        return false;

    }

    public static List<Assembly> GetAllReferencedAssemblies(this Assembly _self)
    {
        var queue = new Queue<AssemblyName>(_self.GetReferencedAssemblies());
        var alreadyProcessed = new HashSet<string>() { _self.FullName! };
        var result = new List<Assembly>() { _self };

        while (queue.Count > 0)
        {
            var name = queue.Dequeue();
            var fullName = name.FullName;

            if (alreadyProcessed.Contains(fullName) || fullName.StartsWith("Microsoft.") || fullName.StartsWith("System."))
                continue;

            alreadyProcessed.Add(fullName);

            try
            {
                var newAssembly = Assembly.Load(name);

                if (newAssembly != null)
                {
                    if (!result.Contains(newAssembly))
                        result.Add(newAssembly);

                    foreach (var innerAssemblyName in newAssembly.GetReferencedAssemblies().EnsureNotNull())
                        queue.Enqueue(innerAssemblyName);
                }

                Debug.WriteLine(name);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        return result;
    }

    public static AssemblyName[] GetAllReferencedAssemblyNames(this Assembly _self)
    {
        var queue = new Queue<AssemblyName>(_self.GetReferencedAssemblies());
        var alreadyProcessed = new HashSet<string>() { _self.FullName! };
        var result = new List<AssemblyName>() { _self.GetName() };

        while (queue.Count > 0)
        {
            var name = queue.Dequeue();
            var fullName = name.FullName;

            if (alreadyProcessed.Contains(fullName) || fullName.StartsWith("Microsoft.") || fullName.StartsWith("System."))
                continue;

            alreadyProcessed.Add(fullName);

            try
            {
                var newAssembly = Assembly.Load(name);

                if (newAssembly != null && newAssembly.GetName() is AssemblyName assemblyName)
                {
                    if (!result.Contains(assemblyName))
                        result.Add(assemblyName);

                    foreach (var innerAssemblyName in newAssembly.GetReferencedAssemblies().EnsureNotNull())
                        queue.Enqueue(innerAssemblyName);
                }

                Debug.WriteLine(name);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        return result.ToArray();
    }
}
