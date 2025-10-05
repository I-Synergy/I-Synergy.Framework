using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Singleton)]
public partial class SignInView
{
    public SignInView(SignInViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}