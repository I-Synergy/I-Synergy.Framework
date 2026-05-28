using ISynergy.Framework.Core.Enumerations;
using System.Globalization;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Converts between Languages enum and RadioButton checked states.
/// Uses English as the default language when the bound value is null or invalid.
/// </summary>
/// <remarks>
/// This converter is designed to work with grouped RadioButton controls for language selection.
/// 
/// Convert behavior:
/// - If the bound value matches this converter's Language property, returns true (RadioButton checked)
/// - If the bound value is null/invalid and this is the English RadioButton, returns true (default)
/// - Otherwise returns false (RadioButton unchecked)
/// 
/// ConvertBack behavior:
/// - If the RadioButton is checked, returns the Language property value
/// - If the RadioButton is unchecked, returns Binding.DoNothing (preserves current binding value)
/// 
/// Example: Each RadioButton should be configured with a separate instance having a specific Language:
/// - RadioButton 1: Language = Languages.English (default if value is null)
/// - RadioButton 2: Language = Languages.Dutch
/// - RadioButton 3: Language = Languages.German
/// - RadioButton 4: Language = Languages.French
/// </remarks>
public class RadioButtonToLanguageConverter : IMarkupExtension<RadioButtonToLanguageConverter>, IValueConverter
{
    /// <summary>
    /// Gets or sets the language this RadioButton represents.
    /// </summary>
    public Languages Language { get; set; }

    /// <summary>
    /// Converts a Languages enum value to a boolean indicating if this RadioButton should be checked.
    /// </summary>
    /// <remarks>
    /// When the bound value is null or not a Languages enum, defaults to true if this converter
    /// is configured for Languages.English, otherwise false.
    /// </remarks>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Languages language)
            return language == Language;

        // Default: English RadioButton is selected when value is null/invalid
        return Language == Languages.English;
    }

    /// <summary>
    /// Converts a RadioButton's checked state back to a Languages enum value.
    /// </summary>
    /// <remarks>
    /// Returns the Language property when checked, or Binding.DoNothing when unchecked
    /// to prevent unchecked RadioButtons from updating the bound value.
    /// </remarks>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isChecked && isChecked)
            return Language;

        return Binding.DoNothing;
    }

    public RadioButtonToLanguageConverter ProvideValue(IServiceProvider serviceProvider) => this;

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
        (this as IMarkupExtension<RadioButtonToLanguageConverter>).ProvideValue(serviceProvider);
}
