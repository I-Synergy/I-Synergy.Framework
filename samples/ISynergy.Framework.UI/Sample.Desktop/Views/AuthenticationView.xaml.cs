using ISynergy.Framework.Mvvm.Abstractions.Views;
using Sample.ViewModels;

namespace Sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AuthenticationView : IAuthenticationView
    {
        public AuthenticationView(AuthenticationViewModel viewModel)
            : base(viewModel)
        {
            this.InitializeComponent();
        }
    }
}
