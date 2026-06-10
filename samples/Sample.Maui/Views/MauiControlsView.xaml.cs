using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Scoped)]
public partial class MauiControlsView
{
    public MauiControlsView(MauiControlsViewModel viewModel)
       : base(viewModel)
    {
        InitializeComponent();
    }
}
