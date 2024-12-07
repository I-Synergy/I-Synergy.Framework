using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Singleton)]
public partial class SignInView
{
    public SignInView(IContext context, SignInViewModel viewModel)
        : base(context, viewModel)
    {
        InitializeComponent();
    }
}