using GalaSoft.MvvmLight.Ioc;
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

            this.PrimaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Ok");
            this.SecondaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Cancel");
        }
    }
}
