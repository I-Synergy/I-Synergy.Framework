using ISynergy.Services;
using ISynergy.ViewModels.Library;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Views.Library
{
    public sealed partial class LanguageWindow
    {
        private LanguageViewModel ViewModel => DataContext as LanguageViewModel;

        public LanguageWindow()
        {
            this.InitializeComponent();
            this.PrimaryButtonText = ActivatorUtilities.CreateInstance<ILanguageService>(ViewModel.BaseService.ServiceProvider).GetString("Generic_Ok");
            this.SecondaryButtonText = ActivatorUtilities.CreateInstance<ILanguageService>(ViewModel.BaseService.ServiceProvider).GetString("Generic_Close");
        }
    }
}
