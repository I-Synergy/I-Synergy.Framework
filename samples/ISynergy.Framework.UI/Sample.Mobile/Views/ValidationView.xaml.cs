using Sample.ViewModels;

namespace Sample.Views;

public partial class ValidationView : IView
{
	public ValidationView(ValidationViewModel viewModel)
		: base(viewModel)
	{
		InitializeComponent();
	}
}