using ISynergy.Framework.Core.Enumerations;
using System.Globalization;
using System.Windows.Data;

namespace ISynergy.Framework.UI.Converters;

public class RadioButtonToLanguageConverter : IValueConverter
{
    private Languages _value;

    public Languages Language { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Languages languages)
            _value = languages;

        return value?.Equals(Language) ?? false;
    }

    public object ConvertBack(object? isChecked, Type targetType, object? parameter, CultureInfo culture)
        => isChecked is bool checkedValue && checkedValue  // Is this the checked RadioButton? If so...
        ? Language          // Send 'Language' back to update the associated binding. Otherwise...
        : _value;           // Return de converterd value, telling the binding 'ignore this change'
}
