using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Framework.UI.Markup
{
    /// <summary>
    /// Class LanguageResource.
    /// Implements the <see cref="MarkupExtension" />
    /// </summary>
    /// <seealso cref="MarkupExtension" />
    [Bindable]
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public class LanguageResource : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <returns>System.Object.</returns>
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
