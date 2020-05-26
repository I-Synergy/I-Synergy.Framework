using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Framework.Windows.Markup
{
    [Bindable]
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public class LanguageResource : MarkupExtension
    {
        public string Key { get; set; }

        protected override object ProvideValue()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                if (DesignMode.DesignMode2Enabled)
                {
                    return $"[{Key}]";
                }
                else
                {
                    return ServiceLocator.Default.GetInstance<ILanguageService>().GetString(Key);
                }
            }
            else
            {
                return "[Empty]";
            }
        }
    }
}
