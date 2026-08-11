using ISynergy.Framework.Core.Serializers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// These extension methods provide a convenient way to store and retrieve objects as preferences using JSON serialization and deserialization.
/// </summary>
public static class PreferencesExtensions
{
    /// <summary>
    /// Sets an object of type T as a preference.
    /// </summary>
    /// <typeparam name="T">The type of the object to be stored as a preference.</typeparam>
    /// <param name="preferences">The preferences storage.</param>
    /// <param name="key">The key under which the object will be stored.</param>
    /// <param name="obj">The object to be stored as a preference.</param>
    /// <param name="sharedName">The shared preferences name (optional).</param>
    /// <remarks>
    /// This overload uses reflection-based JSON serialization and is not AOT-safe.
    /// For AOT-compatible code, use <see cref="SetObject{T}(IPreferences, string, T, JsonTypeInfo{T}, string)"/> instead.
    /// </remarks>
    [RequiresDynamicCode("JsonSerializer.Serialize<T> without a JsonTypeInfo requires runtime code generation for types not covered by System.Text.Json's built-in fast paths; not supported under NativeAOT. Use the SetObject overload that takes a JsonTypeInfo<T> instead.")]
    [RequiresUnreferencedCode("Reflection-based JSON serialization is not trim-compatible. Use the SetObject overload that takes a JsonTypeInfo<T> instead.")]
    public static void SetObject<T>(this IPreferences preferences, string key, T? obj, string? sharedName = null)
        where T : class
    {
        string? jsonValue = null;

        if (obj != null)
            jsonValue = JsonSerializer.Serialize<T>(obj, DefaultJsonSerializers.Web);

        preferences.Set<string?>(key, jsonValue, sharedName);
    }

    /// <summary>
    /// Sets an object of type T as a preference using a source-generated <see cref="JsonTypeInfo{T}"/>.
    /// This overload is fully AOT-safe.
    /// </summary>
    /// <typeparam name="T">The type of the object to be stored as a preference.</typeparam>
    /// <param name="preferences">The preferences storage.</param>
    /// <param name="key">The key under which the object will be stored.</param>
    /// <param name="obj">The object to be stored as a preference.</param>
    /// <param name="typeInfo">
    /// Type metadata for <typeparamref name="T"/> from a source-generated <see cref="System.Text.Json.Serialization.JsonSerializerContext"/>.
    /// </param>
    /// <param name="sharedName">The shared preferences name (optional).</param>
    public static void SetObject<T>(this IPreferences preferences, string key, T? obj, JsonTypeInfo<T> typeInfo, string? sharedName = null)
        where T : class
    {
        string? jsonValue = obj != null ? JsonSerializer.Serialize(obj, typeInfo) : null;
        preferences.Set<string?>(key, jsonValue, sharedName);
    }

    /// <summary>
    /// Gets an object of type T from the preferences.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="preferences"></param>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <param name="sharedName"></param>
    /// <returns></returns>
    /// <remarks>
    /// This overload uses reflection-based JSON deserialization and is not AOT-safe.
    /// For AOT-compatible code, use <see cref="GetObject{T}(IPreferences, string, JsonTypeInfo{T}, T, string)"/> instead.
    /// </remarks>
    [RequiresDynamicCode("JsonSerializer.Deserialize<T> without a JsonTypeInfo requires runtime code generation for types not covered by System.Text.Json's built-in fast paths; not supported under NativeAOT. Use the GetObject overload that takes a JsonTypeInfo<T> instead.")]
    [RequiresUnreferencedCode("Reflection-based JSON deserialization is not trim-compatible. Use the GetObject overload that takes a JsonTypeInfo<T> instead.")]
    public static T? GetObject<T>(this IPreferences preferences, string key, T? defaultValue = null, string? sharedName = null)
        where T : class
    {
        string? jsonValue = preferences.Get<string?>(key, null, sharedName);

        if (jsonValue == null)
            return defaultValue;

        return JsonSerializer.Deserialize<T>(jsonValue, DefaultJsonSerializers.Web);
    }

    /// <summary>
    /// Gets an object of type T from the preferences using a source-generated <see cref="JsonTypeInfo{T}"/>.
    /// This overload is fully AOT-safe.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="preferences"></param>
    /// <param name="key"></param>
    /// <param name="typeInfo">
    /// Type metadata for <typeparamref name="T"/> from a source-generated <see cref="System.Text.Json.Serialization.JsonSerializerContext"/>.
    /// </param>
    /// <param name="defaultValue"></param>
    /// <param name="sharedName"></param>
    /// <returns></returns>
    public static T? GetObject<T>(this IPreferences preferences, string key, JsonTypeInfo<T> typeInfo, T? defaultValue = null, string? sharedName = null)
        where T : class
    {
        string? jsonValue = preferences.Get<string?>(key, null, sharedName);

        if (jsonValue == null)
            return defaultValue;

        return JsonSerializer.Deserialize(jsonValue, typeInfo);
    }
}
