using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Singleton)]
public partial class SignUpView
{
    public SignUpView(SignUpViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}