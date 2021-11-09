using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Abstractions.Services;
using Windows.ApplicationModel;

#if (WINDOWS_UWP || HAS_UNO)
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
#else
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
#endif

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
