using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Views;
using ISynergy.Framework.UI.ViewModels;

namespace Sample.Views;

public partial class RegistrationView : IRegistrationView
{
    public RegistrationView(IContext context)
        : base(context, typeof(RegistrationViewModel))
    {
        InitializeComponent();
    }
}