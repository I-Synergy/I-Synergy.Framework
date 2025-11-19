using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Scoped)]
public partial class ControlsView
{
    public ControlsView(ControlsViewModel viewModel)
       : base(viewModel)
    {
        InitializeComponent();
    }
}