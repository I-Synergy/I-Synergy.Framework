using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.UI.Markup;

/// <summary>
/// Class EnumCollection.
/// Implements the <see cref="MarkupExtension" />
/// </summary>
/// <seealso cref="MarkupExtension" />
[Bindable]
[MarkupExtensionReturnType(ReturnType = typeof(List<KeyValuePair<int, string>>))]
public class EnumKeyValueCollection : MarkupExtension
{
    /// <summary>
    /// Gets or sets the type of the enum.
    /// </summary>
    /// <value>The type of the enum.</value>
    public Type? EnumType { get; set; }

    /// <summary>
    /// Provides the value.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A list of <see cref="KeyValuePair{TKey,TValue}"/> with enum integer values and their localized descriptions.</returns>
    /// <remarks>
    /// Uses <see cref="Enum.GetValues(Type)"/> with a runtime <see cref="Type"/> argument.
    /// This requires the enum type's members to be preserved by the IL trimmer and is not compatible
    /// with NativeAOT. For AOT-safe scenarios, compute enum key-value pairs using
    /// <c>Enum.GetValues&lt;TEnum&gt;()</c> in ViewModel code and bind to the resulting collection.
    /// </remarks>
    [RequiresUnreferencedCode("Enum.GetValues(Type) requires enum type members to be preserved by the trimmer.")]
    [RequiresDynamicCode("Enum.GetValues requires dynamic code generation for runtime type resolution.")]
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
        Argument.IsNotNull(EnumType);

        var list = new List<KeyValuePair<int, string>>();

        if (EnumType!.IsEnum)
        {
            list.AddRange(from Enum item in Enum.GetValues(EnumType) select new KeyValuePair<int, string>(System.Convert.ToInt32(item), GetDescription(item)));
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
        return ServiceLocator.Default.GetRequiredService<ILanguageService>().GetString(value.ToString());
    }
}
