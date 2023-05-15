using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Views;
using ISynergy.Framework.UI.ViewModels;

namespace Sample.Views;

public partial class AuthenticationView : IAuthenticationView
{
    public AuthenticationView(IContext context)
        : base(context, typeof(AuthenticationViewModel))
    {
        InitializeComponent();
    }
}