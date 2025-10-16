using ISynergy.Framework.Core.Enumerations;
using System.Globalization;

namespace ISynergy.Framework.UI.Converters;

public class RadioButtonToThemeConverter : IMarkupExtension<RadioButtonToThemeConverter>, IValueConverter
{
    private Themes _value;

    public Themes Theme { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Themes theme)
            _value = theme;

        return value.Equals(Theme);
    }

    public object ConvertBack(object isChecked, Type targetType, object parameter, CultureInfo culture)
        => (bool)isChecked  // Is this the checked RadioButton? If so...
        ? Theme             // Send 'Theme' back to update the associated binding. Otherwise...
        : _value;           // Return de converterd value, telling the binding 'ignore this change'

    public RadioButtonToThemeConverter ProvideValue(IServiceProvider serviceProvider) => this;

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
        (this as IMarkupExtension<RadioButtonToThemeConverter>).ProvideValue(serviceProvider);
}
