using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Framework.UI.Markup;

/// <summary>
/// Class LanguageResource.
/// Implements the <see cref="MarkupExtension" />
/// </summary>
/// <seealso cref="MarkupExtension" />
[Bindable]
[MarkupExtensionReturnType(ReturnType = typeof(string))]
public partial class LanguageResource : MarkupExtension
{
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    /// <value>The key.</value>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Provides the value.
    /// </summary>
    /// <returns>System.Object.</returns>
    protected override object ProvideValue()
    {
        if (!string.IsNullOrEmpty(Key) && !DesignMode.DesignMode2Enabled)
        {
            return ServiceLocator.Default.GetRequiredService<ILanguageService>().GetString(Key);
        }

        return $"[{Key}]";
    }
}
