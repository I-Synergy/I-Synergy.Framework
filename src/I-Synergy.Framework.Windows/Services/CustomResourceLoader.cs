using Windows.ApplicationModel;
using Windows.UI.Xaml.Resources;

namespace ISynergy.Services
{
    public class CustomResourceLoader : CustomXamlResourceLoader
    {
        private ILanguageService Resources;

        public CustomResourceLoader(ILanguageService languageservice)
        {
            Resources = languageservice;
        }

        protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType)
        {
            if (DesignMode.DesignModeEnabled)
            {
                return "[Empty]";
            }
            else
            {
                if (Resources != null)
                {
                    return Resources.GetString(resourceId);
                }
                else
                {
                    return "[Empty]";
                }
            }
        }
    }
}
