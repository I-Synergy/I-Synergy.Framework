using Microsoft.Maui.Controls.Internals;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Utilities;

#nullable enable

public static class ResourceUtility
{
    private static T? GetResource<T>(ResourceDictionary resources, object? resource)
    {
        //Check resource type
        switch (resource)
        {
            case OnPlatform<T> value:
                //Get return type platform specific resource
                return GetResource<T>(resources, value.Platforms?.FirstOrDefault(p => p.Platform.Contains($"{DeviceInfo.Platform}"))?.Value);

            case OnIdiomExtension value:
                //Get idiom specific resource
                return GetResource<T>(resources, ResolveOnIdiom(value.Default, value.Desktop, value.Phone, value.Tablet, value.TV, value.Watch));

            case OnIdiom<T> value:
                //Get return type idiom specific resource
                return ResolveOnIdiom(value.Default, value.Desktop, value.Phone, value.Tablet, value.TV, value.Watch);

            case DynamicResource value:
                //Get dynamic resource
                return resources.FindResource<T>(value.Key);

            case string value:
                return ConvertFromString<T>(value);

            case T value:
                //Return found resource value
                return value;

            default:
                //Return default resource value
                return default;
        }
    }

    /// <summary>
    /// Selects the value matching the current device idiom, defaulting to <paramref name="defaultValue"/>.
    /// </summary>
    private static T ResolveOnIdiom<T>(T defaultValue, T desktop, T phone, T tablet, T tv, T watch)
    {
        var idiom = DeviceInfo.Idiom;

        if (idiom == DeviceIdiom.Desktop) return desktop;
        if (idiom == DeviceIdiom.Phone) return phone;
        if (idiom == DeviceIdiom.Tablet) return tablet;
        if (idiom == DeviceIdiom.TV) return tv;
        if (idiom == DeviceIdiom.Watch) return watch;

        return defaultValue;
    }

    /// <summary>
    /// Attempts to convert a string resource value to <typeparamref name="T"/>, returning default on failure.
    /// </summary>
    private static T? ConvertFromString<T>(string value)
    {
        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter?.CanConvertFrom(typeof(string)) ?? false)
            {
                // Cast ConvertFromString(string text) : object to (T)
                return (T?)converter.ConvertFromString(value);
            }
            return default;
        }
        catch
        {
            return default;
        }
    }

    public static T? FindResource<T>(this ResourceDictionary resources, string resourceKey)
    {
        //Verify parameter
        if (string.IsNullOrWhiteSpace(resourceKey))
            return default;

        //Attempt to find resource
        if (resources.TryGetValue(resourceKey, out var resource))
            //Get resource
            return GetResource<T>(resources, resource);
        else
            //If nothing found, return default
            return default;
    }

    public static T? FindResource<T>(string resourceKey)
    {
        //Verify Current Application
        if (Application.Current is not null)
            //Attempt to find resource
            return Application.Current.Resources.FindResource<T>(resourceKey);
        else
            //If nothing found, return default
            return default;
    }
}
