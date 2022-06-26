using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Windows.ApplicationModel;

namespace ISynergy.Framework.UI.Markup
{
    /// <summary>
    /// Class LanguageResource.
    /// Implements the <see cref="MarkupExtension" />
    /// </summary>
    /// <seealso cref="MarkupExtension" />
    [Bindable]
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public partial class LanguageResource : MarkupExtension
    {
        public LanguageResource()
            : base()
        {
        }

        public LanguageResource(string key)
            : this()
        {
            Key = key;
        }

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
