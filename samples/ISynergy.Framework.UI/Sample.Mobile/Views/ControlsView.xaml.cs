using Sample.ViewModels;

namespace Sample.Views;

public partial class ControlsView
{ 
	public ControlsView(ControlsViewModel viewModel)
        :base(viewModel)
	{
		InitializeComponent();
	}
}