using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using Sample.ViewModels;

namespace Sample.Views;

[Singleton(true)]
public partial class SignUpView
{
    public SignUpView(IContext context, SignUpViewModel viewModel)
        : base(context, viewModel)
    {
        InitializeComponent();
    }
}