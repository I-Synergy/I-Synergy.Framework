using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using Sample.ViewModels;

namespace Sample.Views;

[Singleton(true)]
public partial class SignInView
{
    public SignInView(IContext context, SignInViewModel viewModel)
        : base(context, viewModel)
    {
        InitializeComponent();
    }
}