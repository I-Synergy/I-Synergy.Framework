using Windows.ApplicationModel;
using Windows.UI.Xaml.Resources;

namespace ISynergy.Services
{
    public class CustomResourceLoader : CustomXamlResourceLoader
    {
        private ILanguageService LanguageService;

        public CustomResourceLoader(ILanguageService languageservice)
        {
            LanguageService = languageservice;
        }

        protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType)
        {
            if (DesignMode.DesignModeEnabled)
            {
                return "[Empty]";
            }
            else
            {
                if (LanguageService != null)
                {
                    return LanguageService.GetString(resourceId);
                }
                else
                {
                    return "[Empty]";
                }
            }
        }
    }
}
