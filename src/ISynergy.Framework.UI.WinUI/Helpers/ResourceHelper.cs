using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using System.Reflection;

namespace ISynergy.Framework.UI.Helpers;

/// <summary>
/// Class ResourceHelper.
/// </summary>
internal static class ResourceHelper
{
    /// <summary>
    /// Gets the resource dictionary by path.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="resourcePath">The resource path.</param>
    /// <returns>ResourceDictionary.</returns>
    public static ResourceDictionary? GetResourceDictionaryByPath(Type type, string resourcePath)
    {
        var assembly = type.GetTypeInfo().Assembly;

        if (assembly is null)
            throw new InvalidOperationException("Assembly not found.");

        using var stream = assembly.GetManifestResourceStream(resourcePath);

        if (stream is null)
            throw new InvalidOperationException("Resource file not found.");

        var reader = new StreamReader(stream);
        var dictionary = XamlReader.Load(reader.ReadToEnd()) as ResourceDictionary;
        return dictionary;
    }

    /// <summary>
    /// Loads the embedded resource.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="resourcePath">The resource path.</param>
    /// <param name="key">The key.</param>
    /// <returns>System.Object.</returns>
    public static object LoadEmbeddedResource(Type type, string resourcePath, object key)
    {
        return GetResourceDictionaryByPath(type, resourcePath)![key];
    }

    /// <summary>
    /// Loads the manifest stream bytes.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="resourcePath">The resource path.</param>
    /// <returns>System.Byte[].</returns>
    public static byte[]? LoadManifestStreamBytes(Type type, string resourcePath)
    {
        var assembly = type.GetTypeInfo().Assembly;

        if (assembly is null)
            throw new InvalidOperationException("Assembly not found.");

        using var stream = assembly.GetManifestResourceStream(resourcePath);

        if (stream is null)
            throw new InvalidOperationException("Resource file not found.");

        var bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);

        return bytes;
    }
}
