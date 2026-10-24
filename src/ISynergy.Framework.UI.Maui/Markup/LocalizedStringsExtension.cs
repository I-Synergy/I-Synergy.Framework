using ISynergy.Framework.Globalization.Services;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Markup;

/// <summary>
/// Markup extension that provides a live-updating binding to <see cref="LocalizedStrings.Default"/> indexer.
/// Used in XAML to bind UI text to localized strings that refresh when the culture changes.
/// </summary>
/// <remarks>
/// This extension is trim and AOT-friendly, enabling compiled bindings in MAUI.
/// Usage in XAML:
/// <code>
/// xmlns:m="clr-namespace:ISynergy.Framework.UI.Markup;assembly=ISynergy.Framework.UI.Maui"
/// &lt;Label Text="{m:LocalizedStrings Settings}" /&gt;
/// </code>
/// or with explicit key property:
/// <code>
/// &lt;Label Text="{m:LocalizedStrings Key=Settings}" /&gt;
/// </code>
/// </remarks>
[Bindable(BindableSupport.Yes)]
[ContentProperty(nameof(Key))]
public class LocalizedStringsExtension : IMarkupExtension<BindingBase>
{
    /// <summary>
    /// Gets or sets the localization resource key.
    /// </summary>
    /// <value>The resource key to resolve from <see cref="LocalizedStrings.Default"/> indexer.</value>
    public required string Key { get; set; }

    public LocalizedStringsExtension()
    {
    }

    public LocalizedStringsExtension(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Provides a Binding that targets the <see cref="LocalizedStrings.Default"/> indexer,
    /// with OneWay mode and property change notification.
    /// </summary>
    /// <param name="serviceProvider">The MAUI service provider (unused).</param>
    /// <returns>A <see cref="Binding"/> configured for live localization updates.</returns>
    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        // A string indexer is addressed as "[key]" in a binding path (not "Item[key]").
        // LocalizedStrings raises PropertyChanged("Item[]"), which refreshes every indexer binding.
        return new Binding
        {
            Source = LocalizedStrings.Default,
            Path = $"[{Key}]",
            Mode = BindingMode.OneWay
        };
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
        ((IMarkupExtension<BindingBase>)this).ProvideValue(serviceProvider);
}
