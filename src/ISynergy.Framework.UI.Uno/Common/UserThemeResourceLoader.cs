using Microsoft.UI.Xaml.Resources;

namespace ISynergy.Framework.UI.Common;

/// <summary>
/// Represents a custom implementation of the <see cref="CustomXamlResourceLoader" /> class that allows users to replace the built-in theme resources via the <see cref="UserThemeResources" /> class.
/// </summary>
public sealed class UserThemeResourceLoader : CustomXamlResourceLoader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserThemeResourceLoader"/> class.
    /// </summary>
    internal UserThemeResourceLoader()
    {
    }

    /// <summary>
    /// Retrieves the paths to the Dark or Light user resource dictionaries, containing replacements of the built-in theme resources.
    /// </summary>
    /// <param name="resourceId">The resource identifier.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="propertyType">Type of the property.</param>
    /// <returns>System.Object.</returns>
    protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType)
    {
        return UserThemeResources.GetUriByPath(resourceId);
    }
}
