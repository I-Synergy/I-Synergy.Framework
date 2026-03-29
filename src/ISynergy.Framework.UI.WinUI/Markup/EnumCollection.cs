using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
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
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A list of enum values for <see cref="EnumType"/>.</returns>
    /// <remarks>
    /// Calls <c>EnumType.ToList()</c> which internally uses <see cref="Enum.GetValues(Type)"/>
    /// with a runtime <see cref="Type"/> argument. This requires the enum type's members to be
    /// preserved by the IL trimmer. For AOT-safe scenarios, populate enum collections using
    /// <c>Enum.GetValues&lt;TEnum&gt;()</c> in ViewModel code.
    /// </remarks>
    [UnconditionalSuppressMessage("Trimming", "IL2046",
        Justification = "MarkupExtension.ProvideValue is an external WinUI SDK base method without RequiresUnreferencedCode; suppressing annotation mismatch.")]
    [UnconditionalSuppressMessage("Trimming", "IL2026",
        Justification = "Enum.GetValues(Type) is intentionally used for XAML markup; callers must ensure enum types are preserved.")]
    [UnconditionalSuppressMessage("Trimming", "IL2072",
        Justification = "EnumType property holds a runtime Type for enum resolution in XAML; callers must ensure the type is preserved.")]
    [UnconditionalSuppressMessage("AOT", "IL3050",
        Justification = "Enum.GetValues requires dynamic code; not AOT-safe by design.")]
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
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
        return ServiceLocator.Default.GetRequiredService<ILanguageService>().GetString(value.ToString());
    }
}