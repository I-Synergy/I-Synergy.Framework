using ISynergy.Framework.Core.Serializers;
using System.Text.Json;

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
    public static void SetObject<T>(this IPreferences preferences, string key, T obj, string? sharedName = null)
    {
        string jsonValue = null;

        if (obj != null)
            jsonValue = JsonSerializer.Serialize<T>(obj, DefaultJsonSerializers.Web());

        preferences.Set<string>(key, jsonValue, sharedName);
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
    public static T GetObject<T>(this IPreferences preferences, string key, T defaultValue, string? sharedName = null)
    {
        string jsonValue = preferences.Get<string>(key, null, sharedName);

        if (jsonValue == null)
            return defaultValue;

        return JsonSerializer.Deserialize<T>(jsonValue, DefaultJsonSerializers.Web());
    }
}
