using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using System;
using System.Text;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.Windows.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is TimeSpan timeSpan)
            {
                var result = new StringBuilder();

                if(timeSpan.Days > 0)
                {
                    result.Append($"{timeSpan.Days} {ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Day_s")}, ");
                }

                result.Append(
                    $"{timeSpan.Hours} {ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Hour_s")} " +
                    $"{ServiceLocator.Default.GetInstance<ILanguageService>().GetString("And")} " +
                    $"{timeSpan.Minutes} {ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Minute_s")}");

                return result.ToString();
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
