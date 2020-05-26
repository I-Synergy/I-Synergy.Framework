using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using System;
using System.Text;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.Windows.Converters
{
    public class ChangeTrackingConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is IModelBase model)
            {
                var result = new StringBuilder();

                var userCreated = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Unknown");
                var userChanged = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Unknown");

                if (!string.IsNullOrEmpty(model.CreatedBy)) userCreated = model.CreatedBy;

                result.AppendLine($"{ServiceLocator.Default.GetInstance<ILanguageService>().GetString("InputFirst")} " +
                    $"{model.CreatedDate.ToLocalTime().ToString("f")} {ServiceLocator.Default.GetInstance<ILanguageService>().GetString("By")} {userCreated}");

                if (model.ChangedDate.HasValue)
                {
                    if (!string.IsNullOrEmpty(model.ChangedBy)) userChanged = model.ChangedBy;

                    result.AppendLine($"{ServiceLocator.Default.GetInstance<ILanguageService>().GetString("InputLast")} " +
                        $"{model.ChangedDate.Value.ToLocalTime().ToString("f")} {ServiceLocator.Default.GetInstance<ILanguageService>().GetString("By")} {userChanged}");
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
