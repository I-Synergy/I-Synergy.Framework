using Windows.ApplicationModel.Resources;

namespace ISynergy.Extensions
{
    public static class ResourceExtensions
    {
        private static ResourceLoader ResLoader = new ResourceLoader();

        public static string GetLocalized(this string resourceKey)
        {
            return ResLoader.GetString(resourceKey);
        }
    }
}
