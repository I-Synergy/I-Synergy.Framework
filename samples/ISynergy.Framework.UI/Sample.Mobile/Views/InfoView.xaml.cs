using Sample.ViewModels;

namespace Sample.Views;

public partial class InfoView
{
	public InfoView(InfoViewModel viewModel)
        :base(viewModel)
	{
		InitializeComponent();
	}
}