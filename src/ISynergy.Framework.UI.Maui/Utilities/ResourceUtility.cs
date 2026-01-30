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
                {
                    //Get idiom specific resource
                    var idiomValue = value.Default;
                    if (DeviceInfo.Idiom == DeviceIdiom.Desktop) idiomValue = value.Desktop;
                    if (DeviceInfo.Idiom == DeviceIdiom.Phone) idiomValue = value.Phone;
                    if (DeviceInfo.Idiom == DeviceIdiom.Tablet) idiomValue = value.Tablet;
                    if (DeviceInfo.Idiom == DeviceIdiom.TV) idiomValue = value.TV;
                    if (DeviceInfo.Idiom == DeviceIdiom.Watch) idiomValue = value.Watch;
                    return GetResource<T>(resources, idiomValue);
                }

            case OnIdiom<T> value:
                {
                    //Get return type idiom specific resource
                    var idiomValue = value.Default;
                    if (DeviceInfo.Idiom == DeviceIdiom.Desktop) idiomValue = value.Desktop;
                    if (DeviceInfo.Idiom == DeviceIdiom.Phone) idiomValue = value.Phone;
                    if (DeviceInfo.Idiom == DeviceIdiom.Tablet) idiomValue = value.Tablet;
                    if (DeviceInfo.Idiom == DeviceIdiom.TV) idiomValue = value.TV;
                    if (DeviceInfo.Idiom == DeviceIdiom.Watch) idiomValue = value.Watch;
                    return idiomValue;
                }

            case DynamicResource value:
                //Get dynamic resource
                return resources.FindResource<T>(value.Key);

            case string value:
                {
                    //Attempt to cast to resource type or return default
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

            case T value:
                //Return found resource value
                return value;

            default:
                //Return default resource value
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
