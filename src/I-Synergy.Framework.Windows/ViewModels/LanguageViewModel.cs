using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Windows.Functions;

namespace ISynergy.Framework.Windows.ViewModels
{
    public class LanguageViewModel : ViewModelDialog<string>, ILanguageViewModel
    {
        public override string Title
        {
            get
            {
                return BaseCommonServices.LanguageService.GetString("Language");
            }
        }

        private readonly LocalizationFunctions _localizationFunctions;

        public LanguageViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            LocalizationFunctions localizationFunctions,
            ILoggerFactory loggerFactory)
            : base(context, commonServices, loggerFactory)
        {
            _localizationFunctions = localizationFunctions;
            
            SelectedItem = BaseCommonServices.ApplicationSettingsService.Culture;
        }

        public override Task SubmitAsync(string e)
        {
            BaseCommonServices.ApplicationSettingsService.Culture = e;
            
            _localizationFunctions.SetLocalizationLanguage(e);
            return base.SubmitAsync(e);
        }
    }
}
