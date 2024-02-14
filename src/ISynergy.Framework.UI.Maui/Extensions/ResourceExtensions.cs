namespace ISynergy.Framework.UI.Extensions;
public static class ResourceExtensions
{
    public static object GetResource(this Application application, string key)
    {
        if (application.Resources.TryGetValue(key, out object result))
            return result;

        return null;
    }
}
