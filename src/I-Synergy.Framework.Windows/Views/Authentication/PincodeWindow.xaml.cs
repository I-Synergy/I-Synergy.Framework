using ISynergy.Services;
using ISynergy.ViewModels.Library;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Views.Authentication
{
    public sealed partial class PincodeWindow : IPincodeWindow
    {
        private PincodeViewModel ViewModel => DataContext as PincodeViewModel;

        public PincodeWindow()
        {
            this.InitializeComponent();
            this.PrimaryButtonText = ActivatorUtilities.CreateInstance<ILanguageService>(ViewModel.BaseService.ServiceProvider).GetString("Generic_Ok");
            this.SecondaryButtonText = ActivatorUtilities.CreateInstance<ILanguageService>(ViewModel.BaseService.ServiceProvider).GetString("Generic_Cancel");
        }
    }
}
