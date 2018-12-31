using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;

namespace ISynergy.Converters
{
    public class BladeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is ObservableCollection<IView>)
            {
                if (value is ObservableCollection<IView> blades && blades.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
