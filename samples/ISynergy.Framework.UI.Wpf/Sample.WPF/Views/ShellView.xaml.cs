using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using ISynergy.Framework.UI.Services;
using Syncfusion.UI.Xaml.NavigationDrawer;

namespace Sample.Views;

/// <summary>
/// Interaction logic for ShellView.xaml
/// </summary>
public partial class ShellView : View, IShellView
{
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Default constructor to initialize the view
    /// </summary>
    public ShellView(INavigationService navigationService)
    {
        InitializeComponent();

        _navigationService = navigationService;

        if (_navigationService is NavigationService service)
            service.Frame = ContentRootFrame;
    }

    private void NavigationView_ItemClicked(object sender, NavigationItemClickedEventArgs e)
    {
        if (e.Item.DataContext is ISynergy.Framework.Core.Models.NavigationItem navigationItem &&
            navigationItem.Command.CanExecute(navigationItem.CommandParameter))
            navigationItem.Command.Execute(navigationItem.CommandParameter);
    }
}
