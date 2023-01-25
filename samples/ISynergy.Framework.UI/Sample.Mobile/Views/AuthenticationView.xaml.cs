using ISynergy.Framework.Mvvm.Abstractions.Views;
using ISynergy.Framework.UI.ViewModels;

namespace Sample.Views;

public partial class AuthenticationView : IAuthenticationView
{
	public AuthenticationView(AuthenticationViewModel viewModel)
		: base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}