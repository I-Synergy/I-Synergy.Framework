using System.Globalization;

namespace ISynergy.Framework.UI.Converters;

public class PageVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (int.TryParse(value.ToString(), out var page) && int.TryParse(parameter.ToString(), out var param))
            return page == param;

        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
