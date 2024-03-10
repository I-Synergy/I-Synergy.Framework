using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using Sample.ViewModels;

namespace Sample.Views;

[Singleton(true)]
public partial class SlideShowView
{
    public SlideShowView(IContext context, SlideShowViewModel viewModel)
       : base(context, viewModel)
    {
        InitializeComponent();
    }
}