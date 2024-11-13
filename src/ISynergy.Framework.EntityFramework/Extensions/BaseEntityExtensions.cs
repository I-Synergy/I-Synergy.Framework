using System.Reflection;

namespace ISynergy.Framework.EntityFramework.Extensions;

/// <summary>
/// Class EntityBaseExtensions.
/// </summary>
public static class EntityBaseExtensions
{
    /// <summary>
    /// Determines whether the specified property name has property.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns><c>true</c> if the specified property name has property; otherwise, <c>false</c>.</returns>
    public static bool HasProperty(this object obj, string propertyName)
    {
        try
        {
            return obj.GetType().GetRuntimeProperty(propertyName) is not null;
        }
        catch (AmbiguousMatchException)
        {
            // ambiguous means there is more than one result,
            // which means: a method with that name does exist
            return true;
        }
    }
}