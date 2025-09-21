using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Framework.UI.Markup;

/// <summary>
/// Class EnumCollection.
/// Implements the <see cref="MarkupExtension" />
/// </summary>
/// <seealso cref="MarkupExtension" />
[Bindable]
[MarkupExtensionReturnType(ReturnType = typeof(List<Enum>))]
public class EnumCollection : MarkupExtension
{
    /// <summary>
    /// Gets or sets the type of the enum.
    /// </summary>
    /// <value>The type of the enum.</value>
    public Type? EnumType { get; set; }

    /// <summary>
    /// Provides the value.
    /// </summary>
    /// <returns>System.Object.</returns>
    protected override object ProvideValue()
    {
        Argument.IsEnumType(EnumType);
        return EnumType!.ToList();
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
        return LanguageService.Default.GetString(value.ToString());
    }
}