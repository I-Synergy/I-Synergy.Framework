using ISynergy.Framework.Core.Services;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Windows.ApplicationModel;

namespace ISynergy.Framework.UI.Markup;

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
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Provides the value.
    /// </summary>
    /// <returns>System.Object.</returns>
    protected override object ProvideValue()
    {
        if (!string.IsNullOrEmpty(Key) && !DesignMode.DesignMode2Enabled)
        {
            return LanguageService.Default.GetString(Key);
        }

        return $"[{Key}]";
    }
}
