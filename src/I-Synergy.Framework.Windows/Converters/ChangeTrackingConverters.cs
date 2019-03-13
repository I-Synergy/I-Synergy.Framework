using GalaSoft.MvvmLight.Ioc;
using ISynergy.Services;
using System;
using System.Text;
using Windows.UI.Xaml.Data;

namespace ISynergy.Converters
{
    public class ChangeTrackingConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is IModelBase model)
            {
                var result = new StringBuilder();

                var userCreated = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Unknown");
                var userChanged = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Unknown");

                if (!string.IsNullOrEmpty(model.CreatedBy)) userCreated = model.CreatedBy;

                result.AppendLine($"{SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_InputFirst")} " +
                    $"{model.CreatedDate.ToLocalTime().ToString("f")} {SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_By")} {userCreated}");

                if(model.ChangedDate.HasValue)
                {
                    if (!string.IsNullOrEmpty(model.ChangedBy)) userChanged = model.ChangedBy;

                    result.AppendLine($"{SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_InputLast")} " +
                        $"{model.ChangedDate.Value.ToLocalTime().ToString("f")} {SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_By")} {userChanged}");
                }

                return result.ToString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
