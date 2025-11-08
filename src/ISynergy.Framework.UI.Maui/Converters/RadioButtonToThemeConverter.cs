using ISynergy.Framework.Core.Enumerations;
using System.Globalization;

namespace ISynergy.Framework.UI.Converters;

public class RadioButtonToThemeConverter : IMarkupExtension<RadioButtonToThemeConverter>, IValueConverter
{
    public Themes Theme { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Themes theme)
            return (theme == Theme);

        return false;
    }

    public object ConvertBack(object? isChecked, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public RadioButtonToThemeConverter ProvideValue(IServiceProvider serviceProvider) => this;

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
        (this as IMarkupExtension<RadioButtonToThemeConverter>).ProvideValue(serviceProvider);
}