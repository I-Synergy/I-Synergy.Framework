using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Scoped)]
public partial class SyncView
{
    public SyncView(IContext context, SyncViewModel viewModel)
       : base(context, viewModel)
    {
        InitializeComponent();
    }
}