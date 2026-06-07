using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Scoped)]
public partial class CustomControlsView
{
    public CustomControlsView(CustomControlsViewModel viewModel)
       : base(viewModel)
    {
        InitializeComponent();
    }
}
