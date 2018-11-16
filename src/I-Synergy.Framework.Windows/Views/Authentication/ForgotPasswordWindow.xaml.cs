using GalaSoft.MvvmLight.Ioc;
using ISynergy.Services;
using ISynergy.ViewModels.Authentication;

namespace ISynergy.Views.Authentication
{
    public sealed partial class ForgotPasswordWindow : IForgotPasswordWindow
    {
        private IForgotPasswordViewModel ViewModel => DataContext as IForgotPasswordViewModel;

        public ForgotPasswordWindow()
        {
            this.InitializeComponent();

            this.PrimaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Ok");
            this.SecondaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Cancel");
        }
    }
}
