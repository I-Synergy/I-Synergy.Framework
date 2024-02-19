using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Markup;

/// <summary>
/// Class LanguageResource.
/// Implements the <see cref="IMarkupExtension" />
/// </summary>
/// <seealso cref="IMarkupExtension" />
[Bindable(BindableSupport.Yes)]
[ContentProperty(nameof(Key))]
public class GetStringExtension : IMarkupExtension<string>
{
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    /// <value>The key.</value>
    public string Key { get; set; }

    public GetStringExtension()
        : base()
    {
    }

    public GetStringExtension(string key)
        : this()
    {
        Key = key;
    }

    public string ProvideValue(IServiceProvider serviceProvider)
    {
        if (!string.IsNullOrEmpty(Key))
        {
            return ServiceLocator.Default.GetInstance<ILanguageService>().GetString(Key);
        }

        return $"[{Key}]";
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
        (this as IMarkupExtension<string>).ProvideValue(serviceProvider);
}
