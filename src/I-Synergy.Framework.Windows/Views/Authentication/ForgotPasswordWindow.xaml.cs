// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using ISynergy.Services;
using ISynergy.ViewModels.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Views.Authentication
{
    public sealed partial class ForgotPasswordWindow : IForgotPasswordWindow
    {
        private IForgotPasswordViewModel ViewModel => DataContext as IForgotPasswordViewModel;

        public ForgotPasswordWindow()
        {
            this.InitializeComponent();

            this.PrimaryButtonText = ActivatorUtilities.CreateInstance<ILanguageService>(ViewModel.SynergyService.ServiceProvider).GetString("Generic_Ok");
            this.SecondaryButtonText = ActivatorUtilities.CreateInstance<ILanguageService>(ViewModel.SynergyService.ServiceProvider).GetString("Generic_Cancel");
        }
    }
}
