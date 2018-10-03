// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using CommonServiceLocator;
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

            this.PrimaryButtonText = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Ok");
            this.SecondaryButtonText = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Cancel");
        }
    }
}
