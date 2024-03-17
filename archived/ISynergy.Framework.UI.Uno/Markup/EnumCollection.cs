using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;

namespace ISynergy.Framework.UI.Markup;

/// <summary>
/// Class EnumCollection.
/// Implements the <see cref="MarkupExtension" />
/// </summary>
/// <seealso cref="MarkupExtension" />
[Bindable]
[MarkupExtensionReturnType(ReturnType = typeof(List<KeyValuePair<int, string>>))]
public class EnumCollection : MarkupExtension
{
    /// <summary>
    /// Gets or sets the type of the enum.
    /// </summary>
    /// <value>The type of the enum.</value>
    public Type EnumType { get; set; }

    /// <summary>
    /// Provides the value.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>System.Object.</returns>
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
        Argument.IsNotNull(EnumType);

        var list = new List<KeyValuePair<int, string>>();

        if (EnumType.IsEnum)
        {
            foreach (Enum item in Enum.GetValues(EnumType))
            {
                list.Add(new KeyValuePair<int, string>(System.Convert.ToInt32(item), GetDescription(item)));
            }
        }

        return list;
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="ArgumentNullException">value</exception>
    private static string GetDescription(Enum value)
    {
        Argument.IsNotNull(value);
        return ServiceLocator.Default.GetInstance<ILanguageService>().GetString(value.ToString());
    }
}
