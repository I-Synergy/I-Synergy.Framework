using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace ISynergy.Converters
{
    public class ResourceNameToGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (parameter != null && parameter is string)
            {
                return (Geometry)XamlReader.Load(
                        "<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"
                        + Application.Current.Resources[parameter] as string as string + "</Geometry>");
            }
            else
            {
                return Geometry.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ResourceNameToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (parameter != null && parameter is string)
            {
                return Application.Current.Resources[parameter] as string;
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
