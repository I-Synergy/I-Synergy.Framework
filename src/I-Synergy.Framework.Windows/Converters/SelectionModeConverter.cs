using System;
using ISynergy.Framework.Mvvm.Enumerations;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.Windows.Converters
{
    public class SelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is SelectionModes selectionMode && selectionMode == SelectionModes.Multiple)
            {
                return ListViewSelectionMode.Multiple;
            }

            return ListViewSelectionMode.Single;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
