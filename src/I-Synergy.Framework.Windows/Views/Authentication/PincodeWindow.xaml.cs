using CommonServiceLocator;
using ISynergy.Services;
using ISynergy.ViewModels.Library;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Views.Authentication
{
    public sealed partial class PincodeWindow : IWindow
    {
        private PincodeViewModel ViewModel => DataContext as PincodeViewModel;

        public PincodeWindow()
        {
            this.InitializeComponent();

            this.PrimaryButtonText = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Ok");
            this.SecondaryButtonText = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Cancel");
        }
    }
}
