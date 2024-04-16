using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using Sample.ViewModels;

namespace Sample.Views;

[Scoped(true)]
public partial class SyncView
{
    public SyncView(IContext context, SyncViewModel viewModel)
       : base(context, viewModel)
    {
        InitializeComponent();
    }
}