using Sample.ViewModels;

namespace Sample.Views;

public partial class DialogsView
{
	public DialogsView(DialogsViewModel viewModel)
        : base(viewModel)
	{
		InitializeComponent();
	}
}