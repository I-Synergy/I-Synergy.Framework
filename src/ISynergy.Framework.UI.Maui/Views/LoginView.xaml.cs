using ISynergy.Framework.UI.ViewModels;

namespace ISynergy.Framework.UI.Views;

public partial class LoginView
{
	public LoginView(LoginViewModel viewModel)
		: base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}