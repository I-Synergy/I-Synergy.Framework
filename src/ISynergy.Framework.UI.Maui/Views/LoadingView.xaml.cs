using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.UI.ViewModels;

namespace ISynergy.Framework.UI.Views;

[Singleton(true)]
public partial class LoadingView
{
    public LoadingView(IContext context, LoadingViewModel viewModel)
        : base(context, viewModel)
    {
        InitializeComponent();
    }
}