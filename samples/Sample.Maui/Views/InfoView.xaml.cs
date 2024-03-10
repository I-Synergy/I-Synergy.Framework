using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using Sample.ViewModels;

namespace Sample.Views;

[Scoped(true)]
public partial class InfoView
{
    public InfoView(IContext context, InfoViewModel viewModel)
       : base(context, viewModel)
    {
        InitializeComponent();
    }
}