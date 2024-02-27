using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using Sample.ViewModels;

namespace Sample.Views;

[Scoped(true)]
public partial class ControlsView : IView
{
	public ControlsView(IContext context, ControlsViewModel viewModel)
       : base(context, viewModel)
    {
		InitializeComponent();
	}
}