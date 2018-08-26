using ISynergy.Common;
using ISynergy.Helpers;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ISynergy.Converters
{
    public class SolidColorBrushToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Argument.IsNotNull(nameof(value), value);
            return ((SolidColorBrush)value).Color.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IntegerToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            Argument.IsNotNull(nameof(value), value);
            return new SolidColorBrush(ImageHelper.ConvertInteger2Color((int)value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntegerToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            Argument.IsNotNull(nameof(value), value);
            return ImageHelper.ConvertInteger2Color((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            Argument.IsNotNull(nameof(value), value);
            return ImageHelper.ConvertColor2Integer((Color)value);
        }
    }
}
