using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Models;
using Sample.ViewModels;

namespace Sample.Views;

[Singleton(true)]
public partial class SignInView
{
    //public SignInView()
    //{
    //    InitializeComponent();
    //}

    public SignInView(IContext context, SignInViewModel viewModel)
        : base(context, viewModel)
    {
        InitializeComponent();
    }

    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();

    //    if (Context.IsAuthenticated)
    //    {
    //        if (PrimaryItems.Count > 0 && PrimaryItems.First() is NavigationItem navigationItem && navigationItem.Command.CanExecute(navigationItem.CommandParameter))
    //            navigationItem.Command.Execute(navigationItem.CommandParameter);
    //    }
    //}
}