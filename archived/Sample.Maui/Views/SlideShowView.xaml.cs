using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Singleton)]
public partial class SlideShowView
{
    public SlideShowView(IContext context, SlideShowViewModel viewModel)
       : base(context, viewModel)
    {
        InitializeComponent();
    }
}