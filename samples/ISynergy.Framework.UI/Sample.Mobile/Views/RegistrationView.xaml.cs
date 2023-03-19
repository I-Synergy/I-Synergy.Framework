using ISynergy.Framework.Mvvm.Abstractions.Views;
using ISynergy.Framework.UI.ViewModels;

namespace Sample.Views;

public partial class RegistrationView : IRegistrationView
{
    public RegistrationView(RegistrationViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}