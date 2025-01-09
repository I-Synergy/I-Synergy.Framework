using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sample.Messages;


namespace Sample.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ShellView : View, IShellView
{
    private readonly ICommonServices _commonServices;
    private bool _isLoaded = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellView"/> class.
    /// </summary>
    public ShellView(ICommonServices commonServices)
    {
        InitializeComponent();

        _commonServices = commonServices;

        Loaded += OnViewLoaded;
        Unloaded += OnViewUnloaded;
    }

    private async void OnViewLoaded(object sender, RoutedEventArgs e)
    {
        if (!_isLoaded)
        {
            _isLoaded = true;
            await Task.Delay(100);
            MessageService.Default.Send(new ShellLoadedMessage());
        }
    }

    private void OnViewUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnViewLoaded;
        Unloaded -= OnViewUnloaded;
        _isLoaded = false;
    }

    private async void RootNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) =>
        await _commonServices.NavigationService.GoBackAsync();
}
