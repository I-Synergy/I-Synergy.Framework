using ISynergy.Framework.Core.Enumerations;
using System.Globalization;

namespace ISynergy.Framework.UI.Converters;

public class RadioButtonToLanguageConverter : IMarkupExtension<RadioButtonToLanguageConverter>, IValueConverter
{
    public Languages Language { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Languages language)
            return language == Language;

        return Language == Languages.English;
    }

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
