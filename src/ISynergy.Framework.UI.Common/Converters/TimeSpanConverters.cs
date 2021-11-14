#if WINDOWS_UWP
using Windows.UI.Xaml.Data;
#else
using Microsoft.UI.Xaml.Data;
#endif

namespace ISynergy.Framework.UI.Converters
{
    /// <summary>
    /// Class TimeSpanToStringConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class TimeSpanToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan timeSpan)
            {
                var result = new StringBuilder();

                if (timeSpan.Days > 0)
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

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
