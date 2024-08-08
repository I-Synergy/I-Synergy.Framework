using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Singleton)]
public partial class SignUpView
{
    public SignUpView(IContext context, SignUpViewModel viewModel)
        : base(context, viewModel)
    {
        InitializeComponent();
    }
}