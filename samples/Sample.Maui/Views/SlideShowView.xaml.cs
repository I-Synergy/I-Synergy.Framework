using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Singleton)]
public partial class SlideShowView
{
    public SlideShowView(SlideShowViewModel viewModel)
       : base(viewModel)
    {
        InitializeComponent();
    }
}